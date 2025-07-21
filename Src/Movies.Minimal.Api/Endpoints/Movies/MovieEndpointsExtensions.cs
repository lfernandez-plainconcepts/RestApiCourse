namespace Movies.Minimal.Api.Endpoints.Movies;

public static class MovieEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateMovie();
        app.MapGetMovie();
        app.MapGetAllMovies();
        app.MapUpdateMovie();
        //app.MapDeleteMovie();

        return app;
    }
}
