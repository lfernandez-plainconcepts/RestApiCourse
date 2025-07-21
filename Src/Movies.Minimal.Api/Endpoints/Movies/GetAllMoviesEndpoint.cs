using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Movies.Minimal.Api.Auth;
using Movies.Minimal.Api.Cache;
using Movies.Minimal.Api.Mapping;

namespace Movies.Minimal.Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetAllMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] RequestMoviesFilterParams filterParams,
                [AsParameters] RequestPageParams pageParams,
                [AsParameters] RequestSortParams sortParams,
                IMovieService movieService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var filterOptions = filterParams.MapToOptions();
                var userId = httpContext.GetUserId();
                if (userId is not null)
                {
                    filterOptions.WithUser(userId!.Value);
                }

                var pageOptions = pageParams.MapToOptions();
                var sortOptions = sortParams.MapToOptions();

                var movies = await movieService.GetAllAsync(filterOptions, pageOptions, sortOptions, cancellationToken);
                var movieCount = await movieService.GetCountAsync(filterOptions, cancellationToken);

                var response = movies.MapToResponse(pageOptions, movieCount);

                return TypedResults.Ok(response);
            })
            .WithName($"{Name}V1")
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .CacheOutput(CacheConstants.Policies.Movies);

        builder
            .MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] RequestMoviesFilterParams filterParams,
                [AsParameters] RequestPageParams pageParams,
                [AsParameters] RequestSortParams sortParams,
                IMovieService movieService,
                HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var filterOptions = filterParams.MapToOptions();
                var userId = httpContext.GetUserId();
                if (userId is not null)
                {
                    filterOptions.WithUser(userId!.Value);
                }

                var pageOptions = pageParams.MapToOptions();
                var sortOptions = sortParams.MapToOptions();

                var movies = await movieService.GetAllAsync(filterOptions, pageOptions, sortOptions, cancellationToken);
                var movieCount = await movieService.GetCountAsync(filterOptions, cancellationToken);

                var response = movies.MapToResponse(pageOptions, movieCount);

                return TypedResults.Ok(response);
            })
            .WithName($"{Name}V2")
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(2.0)
            .CacheOutput(CacheConstants.Policies.Movies);

        return builder;
    }
}
