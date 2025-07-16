using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = [.. request.Genres]
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = [.. request.Genres]
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Slug = movie.Slug,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            Genres = [.. movie.Genres]
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies,
        int page,
        int pageSize,
        int totalCount)
    {
        return new MoviesResponse
        {
            Items = [.. movies.Select(m => m.MapToResponse())],
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(r => new MovieRatingResponse()
        {
            MovieId = r.MovieId,
            Slug = r.Slug,
            Rating = r.Rating
        });
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            YearOfRelease = request.Year,
            SortBy = SortOptions.Parse(request.SortBy),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid userId)
    {
        options.UserId = userId;
        return options;
    }
}
