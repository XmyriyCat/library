namespace Library.Contracts.Requests.Author;

public class CreateAuthorRequest : BaseDto<CreateAuthorRequest, Data.Models.Author>
{
    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }
}