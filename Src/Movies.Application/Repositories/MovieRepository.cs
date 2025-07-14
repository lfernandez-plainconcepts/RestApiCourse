using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var insertMovieCommand = new CommandDefinition("""
            INSERT INTO movies (id, slug, title, yearofrelease)
            VALUES (@Id, @Slug, @Title, @YearOfRelease);
            """, movie);

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
                    """, new { MovieId = movie.Id, Name = genre });

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

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var deleteMovieGenresCommand = new CommandDefinition("""
            DELETE FROM genres 
            WHERE movieId = @MovieId;
            """, new { MovieId = id });

        var genresDeletionResult = await connection.ExecuteAsync(deleteMovieGenresCommand);
        if (genresDeletionResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        var deleteMovieCommand = new CommandDefinition("""
            DELETE FROM movies 
            WHERE id = @Id;
            """, new { Id = id });

        var movieDeletionResult = await connection.ExecuteAsync(deleteMovieCommand);
        if (genresDeletionResult <= 0)
        {
            transaction.Rollback();
            return false;
        }

        transaction.Commit();
        return true;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var existsQuery = new CommandDefinition("""
            SELECT COUNT(1)
            FROM movies 
            WHERE id = @Id);
            """, new { Id = id });

        var existsCount = await connection.ExecuteScalarAsync<bool>(existsQuery);

        return existsCount;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var moviesQuery = new CommandDefinition("""
            SELECT m.*, string_agg(g.name, ',') AS genres
            FROM movies m 
            LEFT JOIN genres g ON m.id = g.movieId
            GROUP BY id;
            """);

        var queryResult = await connection.QueryAsync(moviesQuery);

        var movies = queryResult.Select(result => new Movie
        {
            Id = result.id,
            Title = result.title,
            YearOfRelease = result.yearofrelease,
            Genres = Enumerable.ToList(result.genres?.Split(','))
        });

        return movies;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movieQuery = new CommandDefinition("""
            SELECT * 
            FROM movies 
            WHERE id = @Id;
            """, new { Id = id });

        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(movieQuery);

        if (movie == null)
        {
            return null;
        }

        var genresQuery = new CommandDefinition("""
            SELECT name 
            FROM genres 
            WHERE movieId = @MovieId;
            """, new { MovieId = id });

        var genres = await connection.QueryAsync<string>(genresQuery);

        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movieQuery = new CommandDefinition("""
            SELECT * 
            FROM movies 
            WHERE slug = @Slug;
            """, new { Slug = slug });

        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(movieQuery);

        if (movie == null)
        {
            return null;
        }

        var genresQuery = new CommandDefinition("""
            SELECT name 
            FROM genres 
            WHERE movieId = @MovieId;
            """, new { MovieId = movie.Id });

        var genres = await connection.QueryAsync<string>(genresQuery);

        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var deleteMovieGenresCommand = new CommandDefinition("""
            DELETE FROM genres 
            WHERE movieId = @MovieId;
            """, new { MovieId = movie.Id });

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
                    """, new { MovieId = movie.Id, Name = genre });

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
            """, movie);

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
