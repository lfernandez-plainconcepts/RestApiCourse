using Microsoft.AspNetCore.OutputCaching;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Cache;
using Movies.Minimal.Api.Mapping;

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
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.Policies.TrustedMember);

        return builder;
    }
}
