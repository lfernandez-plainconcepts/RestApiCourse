
using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository(IDbConnectionFactory dbConnectionFactory) : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<float?> GetRatingAsync(
        Guid movieId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var ratingQuery = new CommandDefinition("""
            SELECT round(avg(r.rating), 1)
            FROM ratings r 
            WHERE r.movieId = @MovieId;              
            """,
            new { MovieId = movieId },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<float?>(ratingQuery);
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(
        Guid movieId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var ratingQuery = new CommandDefinition("""
            SELECT round(avg(rating), 1),
                   (SELECT rating
                    FROM ratings
                    WHERE movieId = @MovieId 
                      AND userId = @UserId
                    LIMIT 1)
            FROM ratings
            WHERE movieId = @MovieId;              
            """,
            new { MovieId = movieId, UserId = userId },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(ratingQuery);
    }

    public async Task<bool> RateMovieAsync(
        Guid movieId,
        Guid userId,
        int rating,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var insertRatingCommand = new CommandDefinition("""
            INSERT INTO ratings (userid, movieid, rating)
            VALUES (@UserId, @MovieId, @Rating)
            ON CONFLICT (userid, movieid) DO UPDATE
              SET rating = @Rating;
            """,
            new { UserId = userId, MovieId = movieId, Rating = rating },
            cancellationToken: cancellationToken);

        var result = await connection.ExecuteAsync(insertRatingCommand);

        return result > 0;
    }
}
