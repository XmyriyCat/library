namespace Library.Contracts.Requests.Book;

public class UpdateBookRequest : BaseDto<UpdateBookRequest, Data.Models.Book>
{
    public string Isbn { get; set; }
    
    public string Title { get; set; }
    
    public string Genre { get; set; }

    public string Description { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public override void AddCustomMappings()
    {
        SetCustomMappings()
            .Map(dest => dest.Author,
                src => new Data.Models.Author
                {
                    Id = src.AuthorId
                });


        SetCustomMappingsInverse()
            .Map(dest => dest.AuthorId, src => src.Author.Id);
    }
}