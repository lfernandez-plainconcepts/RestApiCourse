namespace Movies.Contracts.Requests;

public class RequestSortParams
{
    public static readonly string[] ValidSortFields =
    [
        "sortBy"
    ];

    public required string? SortBy { get; init; }
}
