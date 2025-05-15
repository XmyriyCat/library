using Library.Contracts.Variables;

namespace Library.Contracts.Models;

public class GetAllBooksOptions
{
    public string? Title { get; set; }
    
    public string? Genre { get; set; }

    public string? Author { get; set; }

    public int Page { get; set; } = PagesRequest.DefaultPage;

    public int PageSize { get; set; } = PagesRequest.DefaultPageSize;
}