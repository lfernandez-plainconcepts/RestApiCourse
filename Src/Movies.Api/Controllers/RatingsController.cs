using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
[Route("")]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    private readonly IRatingService _ratingService = ratingService;

    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);

        return result ? Ok() : NotFound();
    }

    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);
        var ratingsResponse = ratings.MapToResponse();

        return Ok(ratingsResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id,
        [FromBody] RateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var result = await _ratingService.RateMovieAsync(
            id,
            userId!.Value,
            request.Rating,
            cancellationToken);

        return result ? Ok() : NotFound();
    }
}
