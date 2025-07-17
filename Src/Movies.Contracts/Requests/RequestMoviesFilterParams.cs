namespace Movies.Contracts.Requests;

public class RequestMoviesFilterParams
{
    public static readonly string[] ValidFilterFields =
    [
        "title",
        "year"
    ];

    public required string? Title { get; init; }

    public required int? Year { get; init; }
}
