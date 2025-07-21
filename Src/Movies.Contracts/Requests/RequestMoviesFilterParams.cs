namespace Movies.Contracts.Requests;

public class RequestMoviesFilterParams
{
    public static readonly string[] ValidFilterFields =
    [
        "title",
        "year"
    ];

    public string? Title { get; init; }

    public int? Year { get; init; }
}
