using Microsoft.AspNetCore.Http;

namespace Library.Contracts.Requests.Book;

public class CreateBookRequest
{
    public string Isbn { get; set; }

    public string Title { get; set; }

    public string Genre { get; set; }

    public string Description { get; set; }

    public Guid AuthorId { get; set; }
    
    public IFormFile Image { get; set; }
}