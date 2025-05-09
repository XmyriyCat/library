using Library.Contracts.Responses.Author;
using Library.Contracts.Responses.User;

namespace Library.Contracts.Responses.Book;

public class BookResponse
{
    public Guid Id { get; set; }

    public string Isbn { get; set; }

    public string Title { get; set; }

    public string Genre { get; set; }

    public string Description { get; set; }

    public AuthorResponse Author { get; set; }

    public UserBookResponse BookOwner { get; set; }
}