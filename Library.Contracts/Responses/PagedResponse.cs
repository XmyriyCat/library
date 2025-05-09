namespace Library.Contracts.Responses;

public class PagedResponse<TResponse>
{
    public IEnumerable<TResponse> Items { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public bool HasNextPage => TotalItems > Page * PageSize;
}