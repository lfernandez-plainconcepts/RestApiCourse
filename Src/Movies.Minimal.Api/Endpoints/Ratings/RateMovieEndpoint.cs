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
                RateMovieRequest request,
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
            .WithName(Name);

        return builder;
    }
}
