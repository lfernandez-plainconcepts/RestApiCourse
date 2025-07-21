using Microsoft.AspNetCore.OutputCaching;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Cache;
using Movies.Minimal.Api.Mapping;

namespace Movies.Minimal.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPut(ApiEndpoints.Movies.Update, async (
                Guid id,
                UpdateMovieRequest request,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();
                var movie = request.MapToMovie(id);
                var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);
                if (updatedMovie is null)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);
                var response = updatedMovie.MapToResponse();

                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.Policies.TrustedMember);

        return builder;
    }
}
