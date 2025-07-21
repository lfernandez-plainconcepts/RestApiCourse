using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Movies.Minimal.Api.Auth;

public class AdminAuthRequirement(IConfiguration configuration) : IAuthorizationHandler, IAuthorizationRequirement
{
    private readonly IConfiguration _configuration = configuration;

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.Claims.Admin, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }

        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeys.HeaderName, out var extractedApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var apiKey = _configuration.GetValue<string>("AuthByApiKey:ApiKey");
        if (apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userId = _configuration.GetValue<string>("AuthByApiKey:UserId");
        var identity = (ClaimsIdentity)httpContext.User.Identity!;

        identity.AddClaim(new Claim(AuthConstants.Claims.UserId, userId!));

        context.Succeed(this);
        return Task.CompletedTask;
    }
}
