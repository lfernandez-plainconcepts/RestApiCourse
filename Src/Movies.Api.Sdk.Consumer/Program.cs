using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;
using System.Text.Json;

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(settings => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (request, cancellationToken) => await settings.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7072"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

try
{
    await GetMovie();

    var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
    {
        Title = "Spiderman 2",
        YearOfRelease = 2002,
        Genres = new[] { "Action" }
    });

    await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
    {
        Title = "Spiderman 2",
        YearOfRelease = 2002,
        Genres = new[] { "Action", "Adventure" }
    });

    await moviesApi.DeleteMovieAsync(newMovie.Id);

    await GetMovies();

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}
catch (ApiException ex)
{
    Console.WriteLine($"Error: {ex.StatusCode} - {ex.Content}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}

async Task GetMovie()
{
    var movie = await moviesApi.GetMovieAsync("national-security-2003", CancellationToken.None);

    Console.WriteLine(JsonSerializer.Serialize(movie));
}

async Task GetMovies()
{
    var movies = await moviesApi.GetMoviesAsync(
       new RequestMoviesFilterParams { Title = null, Year = 2025 },
       new RequestPageParams { Page = 1, PageSize = 10 },
       new RequestSortParams { SortBy = "title" },
       CancellationToken.None);

    foreach (var movie in movies.Items)
    {
        Console.WriteLine(JsonSerializer.Serialize(movie));
    }
}