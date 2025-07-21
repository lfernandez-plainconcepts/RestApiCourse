using Movies.Api;
using Movies.Api.Auth;
using Movies.Application.Repositories;

namespace Movies.Minimal.Api.Endpoints.Ratings;

public static class DeleteRateEndpoint
{
    public const string Name = "DeleteRate";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete(ApiEndpoints.Movies.DeleteRating, async (
                Guid id,
                IRatingService ratingService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var userId = httpContext.GetUserId();

                var result = await ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);

                return result ? Results.Ok() : Results.NotFound();
            })
            .WithName(Name);

        return builder;
    }
}
