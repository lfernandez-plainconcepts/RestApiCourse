using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
[Route("")]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    private readonly IRatingService _ratingService = ratingService;

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
