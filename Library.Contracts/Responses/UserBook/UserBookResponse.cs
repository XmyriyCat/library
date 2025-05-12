namespace Library.Contracts.Responses.UserBook;

public class UserBookResponse
{
    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    public DateTime TakenDate { get; set; }

    public DateTime ReturnDate { get; set; }
}