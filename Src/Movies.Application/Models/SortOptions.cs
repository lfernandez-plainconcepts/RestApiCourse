namespace Movies.Application.Models;

public class SortOptions
{
    public const char DescendingMoniker = '-';
    public static readonly string[] ValidSortFields =
    [
        "title",
        "year",
    ];

    public string? Field { get; init; }

    public SortDirection Direction { get; init; }

    public static SortOptions? Parse(string? requestParam)
    {
        if (string.IsNullOrEmpty(requestParam))
            return null;

        return new SortOptions
        {
            Field = ExtractSortField(requestParam),
            Direction = ExtractSortOrder(requestParam)
        };
    }

    private static string ExtractSortField(string requestParam)
    {
        return requestParam
           .TrimStart(DescendingMoniker)
           .Trim()
           .ToLowerInvariant();
    }

    private static SortDirection ExtractSortOrder(string requestParam)
    {
        char directionMoniker = requestParam[0];

        return directionMoniker switch
        {
            DescendingMoniker => SortDirection.Descending,
            _ => SortDirection.Ascending
        };
    }
}
