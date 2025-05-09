using System.Linq.Expressions;
using FluentValidation;
using Library.Application.Services.Contracts;
using Library.Contracts.Requests;
using Library.Contracts.Requests.Author;
using Library.Contracts.Responses.Author;
using Library.Contracts.Responses.Book;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using MapsterMapper;

namespace Library.Application.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<Author> _authorValidator;
    private readonly IValidator<PagedRequest> _pagedRequestValidator;

    public AuthorService(IRepositoryWrapper repositoryWrapper, IMapper mapper,
        IValidator<Author> authorValidator, IValidator<PagedRequest> pagedRequestValidator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _authorValidator = authorValidator;
        _pagedRequestValidator = pagedRequestValidator;
    }

    public async Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken token = default)
    {
        var author = _mapper.Map<Author>(request);

        await _authorValidator.ValidateAndThrowAsync(author, token);

        var createdAuthor = await _repositoryWrapper.Authors.CreateAsync(author, token);

        await _repositoryWrapper.SaveChangesAsync(token);

        var response = _mapper.Map<AuthorResponse>(createdAuthor);

        return response;
    }

    public async Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Authors.GetByIdAsync(id, token);

        if (result is null)
        {
            return null;
        }

        var response = _mapper.Map<AuthorResponse>(result);

        return response;
    }

    public async Task<AuthorsResponse> GetAllAsync(PagedRequest request, CancellationToken token = default)
    {
        await _pagedRequestValidator.ValidateAndThrowAsync(request, token);

        var result =
            await _repositoryWrapper.Authors.GetAllPaginationAsync(request.Page, request.PageSize, token: token);

        var response = _mapper.Map<AuthorsResponse>(result);
        
        response.Page = request.Page;
        response.PageSize = request.PageSize;
        response.TotalItems = await _repositoryWrapper.Authors.CountAsync(token);

        return response;
    }

    public async Task<AuthorResponse> UpdateAsync(Guid authorId, UpdateAuthorRequest request,
        CancellationToken token = default)
    {
        var author = _mapper.Map<Author>(request);
        author.Id = authorId;

        await _authorValidator.ValidateAndThrowAsync(author, token);

        var result = await _repositoryWrapper.Authors.UpdateAsync(author, token);
        
        await _repositoryWrapper.SaveChangesAsync(token);

        return _mapper.Map<AuthorResponse>(result);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Authors.DeleteByIdAsync(id, token);
        
        await _repositoryWrapper.SaveChangesAsync(token);
        
        return result;
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.CountAsync(token);
    }

    public async Task<bool> AnyAsync(Expression<Func<Author, bool>> predicate, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.AnyAsync(predicate, token);
    }

    public Task<IEnumerable<BookResponse>> GetAllBooksAsync(Guid authorId, CancellationToken token = default)
    {
        // TODO-3: Implement it

        //return await _repositoryWrapper.Authors.GetAllBooksAsync(authorId, token);

        throw new NotImplementedException();
    }
}