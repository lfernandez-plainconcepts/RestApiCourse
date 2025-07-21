using Microsoft.AspNetCore.Mvc;
using Movies.Api;
using Movies.Api.Auth;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Minimal.Api.Endpoints.Ratings;

public static class RateMovieEndpoint
{
    public const string Name = "RateMovie";

    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPut(ApiEndpoints.Movies.Rate, async (
                Guid id,
                [FromBody] RateMovieRequest request,
                IRatingService ratingService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();

                var result = await ratingService.RateMovieAsync(
                    id,
                    userId!.Value,
                    request.Rating,
                    cancellationToken);

                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization();

        return builder;
    }
}
