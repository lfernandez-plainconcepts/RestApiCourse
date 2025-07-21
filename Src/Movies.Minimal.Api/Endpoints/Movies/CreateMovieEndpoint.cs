using Microsoft.AspNetCore.OutputCaching;
using Movies.Api;
using Movies.Api.Auth;
using Movies.Api.Cache;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Minimal.Api.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPost(ApiEndpoints.Movies.Create, async (
                CreateMovieRequest request,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();
                var movie = request.MapToMovie();

                await movieService.CreateAsync(movie, userId, cancellationToken);
                await outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);

                var response = movie.MapToResponse();

                return TypedResults.CreatedAtRoute(
                    response,
                    GetMovieEndpoint.Name,
                    new { idOrSlug = movie.Id });
            })
            .WithName(Name);

        return builder;
    }
}
