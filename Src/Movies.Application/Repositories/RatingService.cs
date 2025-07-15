
using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    private readonly IRatingRepository _ratingRepository = ratingRepository;
    private readonly IMovieRepository _movieRepository = movieRepository;

    public Task<bool> DeleteRatingAsync(
        Guid movieId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, cancellationToken);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, cancellationToken);
    }

    public async Task<bool> RateMovieAsync(
        Guid movieId,
        Guid userId,
        int rating,
        CancellationToken cancellationToken = default)
    {
        if (rating <= 0 || rating > 5)
        {
            throw new ValidationException(
            [
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5."
                }
            ]);
        }

        var movieExists = await _movieRepository.ExistsByIdAsync(movieId, cancellationToken);

        if (!movieExists)
        {
            return false;
        }

        return await _ratingRepository.RateMovieAsync(movieId, userId, rating, cancellationToken);
    }
}
