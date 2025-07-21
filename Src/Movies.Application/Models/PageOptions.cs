using Movies.Contracts.Requests;

namespace Movies.Application.Models;

public class PageOptions(
    int? page = RequestPageParams.DefaultPage,
    int? pageSize = RequestPageParams.DefaultPageSize)
{
    public int Page { get; init; } = page ?? RequestPageParams.DefaultPage;

    public int PageSize { get; init; } = pageSize ?? RequestPageParams.DefaultPageSize;

    public int PageOffset => PageSize * (Page - 1);
}
