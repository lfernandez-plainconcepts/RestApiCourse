using Dapper;

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
    }
}
