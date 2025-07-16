namespace Movies.Application.Models;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }

    public int? YearOfRelease { get; set; }

    public Guid? UserId { get; set; }

    public SortOptions? SortBy { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
