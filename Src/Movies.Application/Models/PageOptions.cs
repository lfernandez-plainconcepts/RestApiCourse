namespace Movies.Application.Models;

public class PageOptions(
    int? page = PageOptions.DefaultPage,
    int? pageSize = PageOptions.DefaultPageSize)
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;

    public int Page { get; init; } = page ?? DefaultPage;

    public int PageSize { get; init; } = pageSize ?? DefaultPageSize;

    public int PageOffset => PageSize * (Page - 1);
}
