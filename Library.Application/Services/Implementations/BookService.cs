using System.Linq.Expressions;
using FluentValidation;
using Library.Application.Exceptions;
using Library.Application.Services.Contracts;
using Library.Application.Variables;
using Library.Contracts.Models;
using Library.Contracts.Requests.Book;
using Library.Contracts.Responses.Book;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using DirectoryNotFoundException = Library.Application.Exceptions.DirectoryNotFoundException;

namespace Library.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<Book> _bookValidator;
    private readonly IValidator<BooksRequest> _pagedRequestValidator;
    private readonly IConfiguration _config;
    private readonly IDistributedCache _cache;

    public BookService(IRepositoryWrapper repositoryWrapper, IMapper mapper,
        IValidator<Book> bookValidator, IValidator<BooksRequest> pagedRequestValidator,
        IConfiguration config, IDistributedCache cache)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _bookValidator = bookValidator;
        _pagedRequestValidator = pagedRequestValidator;
        _config = config;
        _cache = cache;
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken token = default)
    {
        var book = _mapper.Map<Book>(request);

        await _bookValidator.ValidateAndThrowAsync(book, token);

        var author = await _repositoryWrapper.Authors.GetByIdAsync(request.AuthorId, token);

        if (author is null)
        {
            throw new AuthorIdNotFoundException($"Author is not found. Author Id: '{request.AuthorId}'");
        }

        book.Author = author;

        var createdBook = await _repositoryWrapper.Books.CreateAsync(book, token);

        await UploadImageAsync(request.Image, createdBook.Id, token);

        await _repositoryWrapper.SaveChangesAsync(token);

        var response = _mapper.Map<BookResponse>(createdBook);

        return response;
    }

    public async Task<BookResponse?> GetByIdOrIsbnAsync(string idOrIsbn, CancellationToken token = default)
    {
        var result = Guid.TryParse(idOrIsbn, out var id);

        return result ? await GetByIdAsync(id, token) : await GetByIsbnAsync(idOrIsbn, token);
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Books.GetByIdAsync(id, token);

        if (result is null)
        {
            return null;
        }

        var response = _mapper.Map<BookResponse>(result);

        return response;
    }

    public async Task<BookResponse?> GetByIsbnAsync(string isbn, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Books.GetByIsbnAsync(isbn, token);

        if (result is null)
        {
            return null;
        }

        var response = _mapper.Map<BookResponse>(result);

        return response;
    }

    public async Task<BooksResponse> GetAllAsync(BooksRequest request, CancellationToken token = default)
    {
        await _pagedRequestValidator.ValidateAndThrowAsync(request, token);

        var options = _mapper.Map<GetAllBooksOptions>(request);

        Expression<Func<Book, bool>>? filterPredication = ConfigureBooksFiltering(options);

        var result = await _repositoryWrapper.Books.GetAllPaginationAsync
        (
            options.Page,
            options.PageSize,
            filterPredication,
            token
        );

        var response = _mapper.Map<BooksResponse>(result);

        response.Page = request.Page;
        response.PageSize = request.PageSize;
        response.TotalItems = await _repositoryWrapper.Books.CountAsync(token);

        return response;
    }

    private Expression<Func<Book, bool>>? ConfigureBooksFiltering(GetAllBooksOptions options)
    {
        Expression<Func<Book, bool>> filterByGenre = book => book.Genre.ToLower() == options.Genre!.ToLower();

        Expression<Func<Book, bool>> filterByAuthor = book => book.Author.Name.ToLower() == options.Author!.ToLower();

        if (options.Genre != null)
        {
            return filterByGenre;
        }

        if (options.Author != null)
        {
            return filterByAuthor;
        }

        return null;
    }

    public async Task<BookResponse> UpdateAsync(Guid bookId, UpdateBookRequest request,
        CancellationToken token = default)
    {
        var book = _mapper.Map<Book>(request);

        await _bookValidator.ValidateAndThrowAsync(book, token);

        var author = await _repositoryWrapper.Authors.GetByIdAsync(request.AuthorId, token);

        if (author is null)
        {
            throw new AuthorIdNotFoundException($"Author is not found. Author Id: '{request.AuthorId}'");
        }

        var fileExtension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
        var fileName = bookId + fileExtension;

        book.Author = author;
        book.Id = bookId;
        book.ImageName = fileName;

        var updatedBook = await _repositoryWrapper.Books.UpdateAsync(book, token);

        await DeleteImageAsync(updatedBook.Id, token);

        await UploadImageAsync(request.Image, updatedBook.Id, token);

        await _repositoryWrapper.SaveChangesAsync(token);

        var response = _mapper.Map<BookResponse>(updatedBook);

        return response;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Books.DeleteByIdAsync(id, token);

        await DeleteImageAsync(id, token);

        await _repositoryWrapper.SaveChangesAsync(token);

        return result;
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.CountAsync(token);
    }

    public async Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.AnyAsync(predicate, token);
    }

    public async Task<ImageResult?> GetBookImageByIdOrIsbnAsync(string idOrIsbn, CancellationToken token = default)
    {
        var book = await GetByIdOrIsbnAsync(idOrIsbn, token);

        if (book is null)
        {
            return null;
        }

        var image = await GetImageAsync(book.ImageName, token);

        return image;
    }

    private async Task UploadImageAsync(IFormFile image, Guid fileId, CancellationToken token = default)
    {
        if (image is null || image.Length == 0)
        {
            throw new WrongImageException("Your image is empty.");
        }

        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!SupportedImageExtensions.AllowedExtensions.Contains(fileExtension))
        {
            throw new InvalidImageExtensionException("Unsupported file type.");
        }

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), _config["RootDirectories:Books"]!);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var fileName = fileId + fileExtension;

        var filePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(filePath))
        {
            throw new ImageAlreadyExistsException($"A file with the same name already exists. Id: '{fileId}'");
        }

        await using var fileStream = new FileStream(filePath, FileMode.Create);

        await image.CopyToAsync(fileStream, token);
    }

    private async Task<bool> DeleteImageAsync(Guid fileId, CancellationToken token)
    {
        if (fileId == Guid.Empty)
        {
            throw new FileIdEmptyException("Invalid fileId. The provided GUID is empty.");
        }

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), _config["RootDirectories:Books"]!);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory {directoryPath} does not exist.");
        }

        var matchingFiles = Directory.GetFiles(directoryPath)
            .Where(file => string.Equals(Path.GetFileNameWithoutExtension(file), fileId.ToString(),
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        var deleted = false;

        foreach (var filePath in matchingFiles)
        {
            try
            {
                File.Delete(filePath);
                
                var fileName = Path.GetFileName(filePath);
                    
                await DeleteImageFromCache(fileName, token);
                deleted = true;
            }
            catch (IOException ex)
            {
                throw new DeleteImageException($"Failed to delete file with ID '{fileId}': {ex.Message}");
            }
        }

        return deleted;
    }

    private async Task<ImageResult?> GetImageAsync(string fileName, CancellationToken token)
    {
        var cachedImage = await GetCachedImage(fileName, token);

        if (cachedImage is not null)
        {
            return cachedImage;
        }

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), _config["RootDirectories:Books"]!);

        if (!Directory.Exists(directoryPath))
        {
            return null;
        }
        
        var imageFile = Directory.GetFiles(directoryPath)
            .FirstOrDefault(file => Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase));

        if (imageFile is null)
        {
            return null;
        }

        await using var fileStream = new FileStream(imageFile, FileMode.Open, FileAccess.Read);
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream, token);

        var result = new ImageResult
        {
            ImageBytes = memoryStream.ToArray(),
            MimeType = SupportedImageExtensions.GetMimeType(imageFile)
        };

        await SaveImageToCache(result, fileName, token);

        return result;
    }

    private async Task<ImageResult?> GetCachedImage(string fileName, CancellationToken token)
    {
        var cacheKeyPrefix = _config["Cache:BookImageKeyPrefix:BookImage"];
        var mimeKeyPrefix = _config["Cache:BookImageKeyPrefix:Mime"];

        var imageCacheKey = $"{cacheKeyPrefix}{fileName}";
        var mimeCacheKey = $"{mimeKeyPrefix}{fileName}";

        var cachedImage = await _cache.GetAsync(imageCacheKey, token);
        var cachedMime = await _cache.GetStringAsync(mimeCacheKey, token);

        if (cachedImage == null || string.IsNullOrEmpty(cachedMime))
        {
            return null;
        }

        return new ImageResult
        {
            ImageBytes = cachedImage,
            MimeType = cachedMime
        };
    }

    private async Task SaveImageToCache(ImageResult image, string fileName, CancellationToken token)
    {
        var cacheKeyPrefix = _config["Cache:BookImageKeyPrefix:BookImage"];
        var mimeKeyPrefix = _config["Cache:BookImageKeyPrefix:Mime"];

        var imageCacheKey = $"{cacheKeyPrefix}{fileName}";
        var mimeCacheKey = $"{mimeKeyPrefix}{fileName}";

        var cacheLifeTimeMinutes = int.Parse(_config["Cache:BookImageKeyPrefix:CacheLifeTimeMinutes"]!);

        await _cache.SetAsync(imageCacheKey, image.ImageBytes!, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheLifeTimeMinutes)
        }, token);

        await _cache.SetStringAsync(mimeCacheKey, image.MimeType!, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheLifeTimeMinutes)
        }, token);
    }

    private async Task DeleteImageFromCache(string fileName, CancellationToken token)
    {
        var cacheKeyPrefix = _config["Cache:BookImageKeyPrefix:BookImage"];
        var mimeKeyPrefix = _config["Cache:BookImageKeyPrefix:Mime"];

        var imageCacheKey = $"{cacheKeyPrefix}{fileName}";
        var mimeCacheKey = $"{mimeKeyPrefix}{fileName}";

        await _cache.RemoveAsync(imageCacheKey, token);
        await _cache.RemoveAsync(mimeCacheKey, token);
    }
}