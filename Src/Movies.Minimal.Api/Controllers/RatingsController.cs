//using Asp.Versioning;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Movies.Api.Auth;
//using Movies.Api.Mapping;
//using Movies.Application.Repositories;
//using Movies.Contracts.Requests;
//using Movies.Contracts.Responses;

//namespace Movies.Api.Controllers;


//[ApiController]
//[ApiVersion(1.0)]
//[Authorize]
//[Route("")]
//public class RatingsController(IRatingService ratingService) : ControllerBase
//{
//    private readonly IRatingService _ratingService = ratingService;

//    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
//        CancellationToken cancellationToken)
//    {
//        var userId = HttpContext.GetUserId();

//        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);

//        return result ? Ok() : NotFound();
//    }

//    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
//    [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]
//    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
//    {
//        var userId = HttpContext.GetUserId();

//        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);
//        var ratingsResponse = ratings.MapToResponse();

//        return Ok(ratingsResponse);
//    }

//    [HttpPut(ApiEndpoints.Movies.Rate)]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> RateMovie([FromRoute] Guid id,
//        [FromBody] RateMovieRequest request,
//        CancellationToken cancellationToken)
//    {
//        var userId = HttpContext.GetUserId();

//        var result = await _ratingService.RateMovieAsync(
//            id,
//            userId!.Value,
//            request.Rating,
//            cancellationToken);

//        return result ? Ok() : NotFound();
//    }
//}
