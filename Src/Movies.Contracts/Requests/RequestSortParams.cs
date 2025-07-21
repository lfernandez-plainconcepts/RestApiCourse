namespace Movies.Contracts.Requests;

public class RequestSortParams
{
    public static readonly string[] ValidSortFields =
    [
        "sortBy"
    ];

    public string? SortBy { get; init; }
}
