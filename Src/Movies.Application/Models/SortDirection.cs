namespace Movies.Application.Models;

public enum SortDirection
{
    Ascending,
    Descending
}

public static class SortDirectionExtensions
{
    public static string ToSqlMoniker(this SortDirection direction)
    {
        return direction switch
        {
            SortDirection.Ascending => "ASC",
            SortDirection.Descending => "DESC",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}