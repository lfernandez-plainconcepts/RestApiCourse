﻿using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IRatingService
{
    Task<bool> DeleteRatingAsync(
        Guid movieId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(
       Guid userId,
       CancellationToken cancellationToken = default);

    Task<bool> RateMovieAsync(
        Guid movieId,
        Guid userId,
        int rating,
        CancellationToken cancellationToken = default);
}
