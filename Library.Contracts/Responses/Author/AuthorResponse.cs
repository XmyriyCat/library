using Library.Contracts.Responses.Book;

namespace Library.Contracts.Responses.Author;

public class AuthorResponse : BaseDto<AuthorResponse, Data.Models.Author>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }

    public IEnumerable<BookResponse> Books { get; set; } = [];
}