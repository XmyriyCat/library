namespace Library.Contracts.Requests.Book;

public class BooksRequest : PagedRequest
{
    public string? Author { get; set; }

    public string? Genre { get; set; }
}