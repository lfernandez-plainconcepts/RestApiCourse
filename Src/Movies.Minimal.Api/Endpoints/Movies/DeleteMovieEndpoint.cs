using Microsoft.AspNetCore.OutputCaching;
using Movies.Api;
using Movies.Api.Cache;
using Movies.Application.Services;

namespace Movies.Minimal.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete(ApiEndpoints.Movies.Delete, async (
                Guid id,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                CancellationToken cancellationToken) =>
            {
                var deleted = await movieService.DeleteByIdAsync(id, cancellationToken);
                if (!deleted)
                {
                    return Results.NotFound();
                }
                await outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);

                return TypedResults.Ok();
            })
            .WithName(Name);

        return builder;
    }
}
