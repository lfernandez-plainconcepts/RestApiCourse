namespace Movies.Contracts.Requests;

public class RequestPageParams
{
    public static readonly string[] ValidPageFields =
    [
        "page",
        "pageSize"
    ];

    public required int? Page { get; init; }

    public required int? PageSize { get; init; }
}
