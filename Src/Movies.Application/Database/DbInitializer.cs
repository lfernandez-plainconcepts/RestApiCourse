using Dapper;
using Movies.Application.Models;
using System.Reflection;
using System.Text.Json;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS movies (
                id UUID PRIMARY KEY,
                slug TEXT NOT NULL UNIQUE,
                title TEXT NOT NULL,           
                yearofrelease INTEGER NOT NULL
            );
         """);

        await connection.ExecuteAsync("""
            CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx ON movies USING BTREE(slug);
         """);

        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS genres (
                movieId UUID REFERENCES movies (id),
                name TEXT NOT NULL                
            );
         """);

        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS ratings (
                userid UUID,
                movieId UUID REFERENCES movies (id),                
                rating INTEGER NOT NULL,
                PRIMARY KEY (userid, movieId)
            );
         """);
    }

    public async Task SeedAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movies = await GetMovies();

        foreach (var movie in movies)
        {
            await connection.ExecuteAsync("""
                 INSERT INTO movies (id, slug, title, yearofrelease)
                 VALUES (@Id, @Slug, @Title, @YearOfRelease)
                 ON CONFLICT (id) DO UPDATE
                 SET slug = EXCLUDED.slug,
                     title = EXCLUDED.title,
                     yearofrelease = EXCLUDED.yearofrelease;
                 """, movie);
        }
    }

    private static async Task<List<Movie>> GetMovies()
    {
        string? assemblyLocation = Assembly.GetExecutingAssembly().Location;
        if (string.IsNullOrEmpty(assemblyLocation))
        {
            throw new InvalidOperationException("Assembly location could not be determined.");
        }

        string seedFilesFolderPath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"Database");
        var path = Path.Combine(seedFilesFolderPath, "movies.json");
        var json = await File.ReadAllTextAsync(path);

        var movies = JsonSerializer.Deserialize<List<Movie>>(json);

        if (movies is null)
        {
            return [];
        }

        return movies;
    }
}
