namespace Library.Contracts.Requests.Author;

public class UpdateAuthorRequest : BaseDto<UpdateAuthorRequest, Data.Models.Author>
{
    public string Name { get; set; }

    public string Country { get; set; }

    public DateTime DateOfBirth { get; set; }
}