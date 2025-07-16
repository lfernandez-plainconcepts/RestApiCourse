namespace Movies.Contracts.Responses;
public class PagedResponse<TResponse>
    where TResponse : class
{
    public required IEnumerable<TResponse> Items { get; init; } = [];

    public required int Page { get; init; } = 1;

    public required int PageSize { get; init; } = 10;

    public required int Total { get; init; }

    public bool HasNextPage => Total > (Page * PageSize);
}
