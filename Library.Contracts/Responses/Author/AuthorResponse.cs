using Library.Contracts.Responses.Book;

namespace Library.Contracts.Responses.Author;

public class AuthorResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }
}