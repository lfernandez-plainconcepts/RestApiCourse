using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(
    IMovieRepository movieRepository,
    IRatingRepository ratingRepository,
    IValidator<Movie> movieValidator,
    IValidator<MoviesFilterOptions> filterOptionsValidator,
    IValidator<PageOptions> pageOptionsValidator,
    IValidator<SortOptions> sortOptionsValidator) : IMovieService
{
    private readonly IMovieRepository _movieRepository = movieRepository;
    private readonly IRatingRepository _ratingRepository = ratingRepository;
    private readonly IValidator<Movie> _movieValidator = movieValidator;
    private readonly IValidator<MoviesFilterOptions> _filterOptionsValidator = filterOptionsValidator;
    private readonly IValidator<PageOptions> _pageOptionsValidator = pageOptionsValidator;
    private readonly IValidator<SortOptions> _sortOptionsValidator = sortOptionsValidator;

    public async Task<bool> CreateAsync(
        Movie movie,
        Guid? userId = default,
        CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateAsync(movie, userId, cancellationToken);
    }

    public Task<bool> DeleteByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        MoviesFilterOptions filterOptions,
        PageOptions pageOptions,
        SortOptions? sortOptions = default,
        CancellationToken cancellationToken = default)
    {
        await _filterOptionsValidator.ValidateAndThrowAsync(filterOptions, cancellationToken);
        await _pageOptionsValidator.ValidateAndThrowAsync(pageOptions, cancellationToken);
        if (sortOptions is not null)
        {
            await _sortOptionsValidator.ValidateAndThrowAsync(sortOptions, cancellationToken);
        }

        return await _movieRepository.GetAllAsync(filterOptions, pageOptions, sortOptions, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(
        Guid id,
        Guid? userId = default,
        CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId = default,
        CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public async Task<int> GetCountAsync(
        MoviesFilterOptions filterOptions,
        CancellationToken cancellationToken = default)
    {
        await _filterOptionsValidator.ValidateAndThrowAsync(filterOptions, cancellationToken);

        return await _movieRepository.GetCountAsync(filterOptions, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(
        Movie movie,
        Guid? userId = default,
        CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);
        if (!movieExists)
        {
            return null;
        }

        await _movieRepository.UpdateAsync(movie, cancellationToken);

        if (!userId.HasValue)
        {
            movie.Rating = await _ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
            return movie;
        }

        var (rating, userRating) = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cancellationToken);
        movie.Rating = rating;
        movie.UserRating = userRating;

        return movie;
    }
}
