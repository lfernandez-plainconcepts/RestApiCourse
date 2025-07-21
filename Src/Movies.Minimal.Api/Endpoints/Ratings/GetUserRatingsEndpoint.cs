using Movies.Api;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Minimal.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                Guid id,
                RateMovieRequest request,
                IRatingService ratingService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();

                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);
                var response = ratings.MapToResponse();

                return Results.Ok(response);
            })
            .WithName(Name);

        return builder;
    }
}
