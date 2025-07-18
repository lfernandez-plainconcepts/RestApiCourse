using System.IdentityModel.Tokens.Jwt;

namespace Movies.Api.Sdk.Consumer;

internal class CachedJwt(string jwt)
{
    private readonly string _cachedJwt = jwt;

    public string GetJwt()
    {
        return _cachedJwt;
    }

    public DateTime GetExpiry()
    {
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedJwt);
        var expiryClaim = jwt.Claims.Single(claim => claim.Type == "exp").Value;
        int epoch = int.Parse(expiryClaim);
        return UnixTimeStampToDateTime(epoch);
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }
}
