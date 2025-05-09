namespace Library.Contracts.Requests.Book;

public class BooksRequest : PagedRequest
{
    // For using [FromQuery]. Meaning '-' is descending, none is ascending order.
    // Examples: -genre, author
    public string? SortBy { get; set; }
    
    public string? Author { get; set; }
    
    public string? Genre { get; set; }
}