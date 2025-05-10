using Library.Contracts.Variables;

namespace Library.Contracts.Requests;

public class PagedRequest
{
    public int Page { get; set; } = PagesRequest.DefaultPage;

    public int PageSize { get; set; } = PagesRequest.DefaultPageSize;
}