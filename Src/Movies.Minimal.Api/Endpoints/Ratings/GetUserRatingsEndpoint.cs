using Movies.Application.Repositories;
using Movies.Contracts.Responses;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Mapping;

namespace Movies.Minimal.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                IRatingService ratingService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();

                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);
                var response = ratings.MapToResponse();

                return Results.Ok(response);
            })
            .WithName(Name)
            .Produces<IEnumerable<MovieRatingResponse>>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization();

        return builder;
    }
}
