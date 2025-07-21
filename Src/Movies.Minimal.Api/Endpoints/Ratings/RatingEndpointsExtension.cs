﻿namespace Movies.Minimal.Api.Endpoints.Ratings;

public static class RatingEndpointsExtension
{
    public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRateMovie();
        //app.MapDeleteRating();
        //app.MapGetUserRatings();

        return app;
    }
}