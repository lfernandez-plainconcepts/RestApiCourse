using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public class ValidationMappingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var validationFailureResponse = new ValidationFailureResponse()
            {
                Errors = ex.Errors.Select(e => new ValidationResponse
                {
                    PropertyName = e.PropertyName,
                    Message = e.ErrorMessage
                })
            };

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}
