﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Cache;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("")]
public class MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore) : ControllerBase
{
    private readonly IMovieService _movieService = movieService;
    private readonly IOutputCacheStore _outputCacheStore = outputCacheStore;

    [Authorize(AuthConstants.Policies.TrustedMember)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, userId, cancellationToken);
        await _outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);

        var response = movie.MapToResponse();

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    [OutputCache]
    [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, cancellationToken)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    // By default, OutputCache doesn't cache authenticated requests.
    //[ServiceFilter(typeof(ApiKeyAuthFilter))]
    [OutputCache(PolicyName = CacheConstants.Policies.Movies)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "title", "year", "sortBy", "page", "pageSize" }, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] RequestMoviesFilterParams filterParams,
        [FromQuery] RequestPageParams pageParams,
        [FromQuery] RequestSortParams sortParams,
        CancellationToken cancellationToken)
    {
        var filterOptions = filterParams.MapToOptions();
        var userId = HttpContext.GetUserId();
        if (userId is not null)
        {
            filterOptions.WithUser(userId!.Value);
        }

        var pageOptions = pageParams.MapToOptions();
        var sortOptions = sortParams.MapToOptions();

        var movies = await _movieService.GetAllAsync(filterOptions, pageOptions, sortOptions, cancellationToken);
        var movieCount = await _movieService.GetCountAsync(filterOptions, cancellationToken);

        var response = movies.MapToResponse(pageOptions, movieCount);
        return Ok(response);
    }

    [Authorize(AuthConstants.Policies.TrustedMember)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);
        if (updatedMovie is null)
        {
            return NotFound();
        }
        await _outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);
        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.Policies.Admin)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        await _outputCacheStore.EvictByTagAsync(CacheConstants.Tags.Movies, cancellationToken);
        return Ok();
    }
}
