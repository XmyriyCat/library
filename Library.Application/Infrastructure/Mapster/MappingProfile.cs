using Library.Contracts.Models;
using Library.Contracts.Requests.Author;
using Library.Contracts.Requests.Book;
using Library.Contracts.Responses.Author;
using Library.Contracts.Responses.Book;
using Library.Contracts.Responses.User;
using Library.Data.Models;
using Mapster;

namespace Library.Application.Infrastructure.Mapster;

public static class MappingProfile
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateAuthorRequest, Author>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

        TypeAdapterConfig<UpdateAuthorRequest, Author>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

        TypeAdapterConfig<Author, AuthorResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

        TypeAdapterConfig<IEnumerable<Author>, AuthorsResponse>.NewConfig()
            .Map(dest => dest.Items, src => src.Select(x => x.Adapt<AuthorResponse>()));

        TypeAdapterConfig<Book, BookResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Isbn, src => src.Isbn)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ImageName, src => src.ImageName)
            .Map(dest => dest.Author, src => src.Author.Adapt<AuthorResponse>())
            .AfterMapping((src, dest) =>
            {
                if (!Equals(src.UserBook, null))
                {
                    dest.BookOwner = src.UserBook.Adapt<UserBookResponse>();
                    dest.BookOwner.User = src.UserBook.User.Adapt<UserResponse>();
                }
            });

        TypeAdapterConfig<CreateBookRequest, Book>.NewConfig()
            .Map(dest => dest.Isbn, src => src.Isbn)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Description, src => src.Description);

        TypeAdapterConfig<UpdateBookRequest, Book>.NewConfig()
            .Map(dest => dest.Isbn, src => src.Isbn)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Genre, src => src.Genre)
            .Map(dest => dest.Description, src => src.Description);

        TypeAdapterConfig<IEnumerable<Book>, BooksResponse>.NewConfig()
            .Map(dest => dest.Items, src => src.Select(x => x.Adapt<BookResponse>()));

        TypeAdapterConfig<BooksRequest, GetAllBooksOptions>.NewConfig();

        TypeAdapterConfig<User, UserResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.Email, src => src.Email);
        
        TypeAdapterConfig<UserBook, UserBookResponse>.NewConfig();
    }
}