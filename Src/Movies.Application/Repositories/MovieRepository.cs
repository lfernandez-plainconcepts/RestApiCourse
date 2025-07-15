using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<bool> CreateAsync(
        Movie movie,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var insertMovieCommand = new CommandDefinition("""
            INSERT INTO movies (id, slug, title, yearofrelease)
            VALUES (@Id, @Slug, @Title, @YearOfRelease);
            """,
            movie,
            cancellationToken: cancellationToken);

        var insertMovieResult = await connection.ExecuteAsync(insertMovieCommand);

        if (insertMovieResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        foreach (var genre in movie.Genres)
        {
            var insertGenreCommand = new CommandDefinition("""
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);
                    """,
                    new { MovieId = movie.Id, Name = genre },
                    cancellationToken: cancellationToken);

            var insertGenreResult = await connection.ExecuteAsync(insertGenreCommand);

            if (insertGenreResult <= 0)
            {
                transaction.Rollback();
                return false;
            }
        }

        transaction.Commit();
        return true;
    }

    public async Task<bool> DeleteByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var deleteMovieGenresCommand = new CommandDefinition("""
            DELETE FROM genres 
            WHERE movieId = @MovieId;
            """,
            new { MovieId = id },
            cancellationToken: cancellationToken);

        var genresDeletionResult = await connection.ExecuteAsync(deleteMovieGenresCommand);
        if (genresDeletionResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        var deleteMovieCommand = new CommandDefinition("""
            DELETE FROM movies 
            WHERE id = @Id;
            """,
            new { Id = id },
            cancellationToken: cancellationToken);

        var movieDeletionResult = await connection.ExecuteAsync(deleteMovieCommand);
        if (movieDeletionResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        transaction.Commit();
        return true;
    }

    public async Task<bool> ExistsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var existsQuery = new CommandDefinition("""
            SELECT COUNT(1)
            FROM movies 
            WHERE id = @Id;
            """,
            new { Id = id },
            cancellationToken: cancellationToken);

        var existsCount = await connection.ExecuteScalarAsync<bool>(existsQuery);

        return existsCount;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var moviesQuery = new CommandDefinition("""
            SELECT m.*, 
                   string_agg(g.name, ',') AS genres,
                   round(avg(r.rating), 1) AS rating,
                   myr.rating AS userrating
            FROM movies m
              LEFT JOIN genres g ON m.id = g.movieId
              LEFT JOIN ratings r ON m.id = r.movieId
              LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
            GROUP BY id, userrating;
            """,
            new { UserId = userId },
            cancellationToken: cancellationToken);

        var queryResult = await connection.QueryAsync(moviesQuery);

        var movies = queryResult.Select(result => new Movie
        {
            Id = result.id,
            Title = result.title,
            YearOfRelease = result.yearofrelease,
            Rating = (float?)result.rating,
            UserRating = (int?)result.userrating,
            Genres = Enumerable.ToList(result.genres?.Split(',')) ?? new List<string>()
        });

        return movies;
    }

    public async Task<Movie?> GetByIdAsync(
        Guid id,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var movieQuery = new CommandDefinition("""
            SELECT m.*, 
                   round(avg(r.rating), 1) AS rating, 
                   myr.rating AS userrating
            FROM movies m 
              LEFT JOIN ratings r ON m.id = r.movieId
              LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
            WHERE id = @Id
            GROUP BY id, userrating;
            """,
            new { Id = id, UserId = userId },
            cancellationToken: cancellationToken);

        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(movieQuery);

        if (movie == null)
        {
            return null;
        }

        var genresQuery = new CommandDefinition("""
            SELECT name 
            FROM genres 
            WHERE movieId = @MovieId;
            """,
            new { MovieId = id },
            cancellationToken: cancellationToken);

        var genres = await connection.QueryAsync<string>(genresQuery);

        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var movieQuery = new CommandDefinition("""
            SELECT m.*, 
                   round(avg(r.rating), 1) AS rating, 
                   myr.rating AS userrating
            FROM movies m 
              LEFT JOIN ratings r ON m.id = r.movieId
              LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
            WHERE slug = @Slug
            GROUP BY id, userrating;
            """,
            new { Slug = slug, UserId = userId },
            cancellationToken: cancellationToken);

        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(movieQuery);

        if (movie == null)
        {
            return null;
        }

        var genresQuery = new CommandDefinition("""
            SELECT name 
            FROM genres 
            WHERE movieId = @MovieId;
            """,
            new { MovieId = movie.Id },
            cancellationToken: cancellationToken);

        var genres = await connection.QueryAsync<string>(genresQuery);

        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<bool> UpdateAsync(
        Movie movie,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var deleteMovieGenresCommand = new CommandDefinition("""
            DELETE FROM genres 
            WHERE movieId = @MovieId;
            """,
            new { MovieId = movie.Id },
            cancellationToken: cancellationToken);

        var genresDeletionResult = await connection.ExecuteAsync(deleteMovieGenresCommand);
        if (genresDeletionResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        foreach (var genre in movie.Genres)
        {
            var insertGenreCommand = new CommandDefinition("""
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);
                    """,
                    new { MovieId = movie.Id, Name = genre },
                    cancellationToken: cancellationToken);

            var insertGenreResult = await connection.ExecuteAsync(insertGenreCommand);

            if (insertGenreResult <= 0)
            {
                transaction.Rollback();
                return false;
            }
        }

        var updateMovieCommand = new CommandDefinition("""
            UPDATE movies 
            SET slug = @Slug, title = @Title, yearOfRelease = @YearOfRelease
            WHERE id = @Id;
            """,
            movie,
            cancellationToken: cancellationToken);

        var result = await connection.ExecuteAsync(updateMovieCommand);

        if (result <= 0)
        {
            transaction.Rollback();
            return false;
        }

        transaction.Commit();
        return true;
    }
}
