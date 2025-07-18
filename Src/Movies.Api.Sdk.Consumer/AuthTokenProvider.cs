using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer;

internal class AuthTokenProvider(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
    private CachedJwt? _cachedToken = null;
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (_cachedToken is not null && _cachedToken.GetExpiry() > DateTime.UtcNow)
        {
            return _cachedToken.GetJwt();
        }

        await Lock.WaitAsync();
        var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
        {
            userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
            email = "admin@movies.com",
            customClaims = new Dictionary<string, object>
            {
                { "admin", true },
                { "trusted_member", true }
            }
        });
        var newToken = await response.Content.ReadAsStringAsync();
        _cachedToken = new CachedJwt(newToken);
        Lock.Release();

        return _cachedToken.GetJwt();
    }
}
