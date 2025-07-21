using Movies.Api;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Minimal.Api.Endpoints.Movies;

public static class GetMovieEndpoint
{
    public const string Name = "GetMovie";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(ApiEndpoints.Movies.Get, async (
                string idOrSlug,
                IMovieService movieService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();

                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, cancellationToken)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

                if (movie is null)
                {
                    return Results.NotFound();
                }

                var response = movie.MapToResponse();

                return TypedResults.Ok(response);
            })
            .WithName(Name);

        return builder;
    }
}
