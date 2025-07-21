using Movies.Application.Services;
using Movies.Contracts.Responses;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Cache;
using Movies.Minimal.Api.Mapping;

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
            .WithName(Name)
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .CacheOutput(CacheConstants.Policies.Movies);

        return builder;
    }
}
