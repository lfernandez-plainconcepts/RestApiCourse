namespace Movies.Application.Models;

public class MoviesFilterOptions
{
    public string? Title { get; set; }

    public int? YearOfRelease { get; set; }

    public Guid? UserId { get; set; }

    public SortOptions? SortBy { get; set; }
}
