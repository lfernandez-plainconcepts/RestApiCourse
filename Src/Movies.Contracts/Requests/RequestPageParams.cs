namespace Movies.Contracts.Requests;

public class RequestPageParams
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;

    public static readonly string[] ValidPageFields =
    [
        "page",
        "pageSize"
    ];

    public int? Page { get; init; } = DefaultPage;

    public int? PageSize { get; init; } = DefaultPageSize;
}
