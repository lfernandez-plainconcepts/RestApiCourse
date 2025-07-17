namespace Movies.Contracts.Requests;

public class RequestPageParams
{
    public required int? Page { get; init; }

    public required int? PageSize { get; init; }
}
