namespace Library.Data.Models;

public class Book
{
    public Guid Id { get; set; }

    public string Isbn { get; set; }

    public string Title { get; set; }

    public string Genre { get; set; }

    public string Description { get; set; }

    public Author Author { get; set; }

    public UserBook UserBook { get; set; }
}