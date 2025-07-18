using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

[Headers("Authorization: Bearer")]
public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(
        string idOrSlug,
        CancellationToken cancellationToken = default);

    [Get(ApiEndpoints.Movies.GetAll)]
    Task<MoviesResponse> GetMoviesAsync(
        RequestMoviesFilterParams filter,
        RequestPageParams pageParams,
        RequestSortParams sortParams,
        CancellationToken cancellationToken = default);

    [Post(ApiEndpoints.Movies.Create)]
    Task<MovieResponse> CreateMovieAsync(
        [Body] CreateMovieRequest request,
        CancellationToken cancellationToken = default);

    [Put(ApiEndpoints.Movies.Update)]
    Task<MovieResponse> UpdateMovieAsync(
        [AliasAs("id")] Guid id,
        [Body] UpdateMovieRequest request,
        CancellationToken cancellationToken = default);

    [Delete(ApiEndpoints.Movies.Delete)]
    Task DeleteMovieAsync(
        [AliasAs("id")] Guid id,
        CancellationToken cancellationToken = default);

    [Post(ApiEndpoints.Movies.Rate)]
    Task RateMovieAsync(
        [AliasAs("id")] Guid id,
        [Body] RateMovieRequest request,
        CancellationToken cancellationToken = default);

    [Delete(ApiEndpoints.Movies.DeleteRating)]
    Task DeleteRatingAsync(
        [AliasAs("id")] Guid id,
        CancellationToken cancellationToken = default);

    [Get(ApiEndpoints.Ratings.GetUserRatings)]
    Task<IEnumerable<MovieRatingResponse>> GetUserRatingsAsync(CancellationToken cancellationToken = default);
}
