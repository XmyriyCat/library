namespace Library.Data.Models;

public class Author
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }
}