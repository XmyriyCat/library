namespace Library.Contracts.Requests.Author;

public class UpdateAuthorRequest
{
    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }
}