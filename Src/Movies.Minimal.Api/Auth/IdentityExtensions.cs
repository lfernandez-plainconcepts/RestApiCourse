namespace Movies.Minimal.Api.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userIdClaim = context.User.Claims
            .SingleOrDefault(c => c.Type == AuthConstants.Claims.UserId);

        if (Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}
