using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(
    IMovieRepository movieRepository,
    IValidator<Movie> movieValidator,
    IRatingRepository ratingRepository,
    IValidator<GetAllMoviesOptions> optionsValidator) : IMovieService
{
    private readonly IMovieRepository _movieRepository = movieRepository;
    private readonly IValidator<Movie> _movieValidator = movieValidator;
    private readonly IRatingRepository _ratingRepository = ratingRepository;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator = optionsValidator;

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
        Guid? userId = default,
        CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        GetAllMoviesOptions options,
        CancellationToken cancellationToken = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken);
        return await _movieRepository.GetAllAsync(options, cancellationToken);
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
