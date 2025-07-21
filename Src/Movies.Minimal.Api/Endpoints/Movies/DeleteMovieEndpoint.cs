using Microsoft.AspNetCore.OutputCaching;
using Movies.Application.Services;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Cache;

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
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.Policies.Admin);

        return builder;
    }
}
