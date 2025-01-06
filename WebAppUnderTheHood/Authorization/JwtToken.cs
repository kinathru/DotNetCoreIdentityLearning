using Newtonsoft.Json;

namespace WebAppUnderTheHood.Authorization;

public class JwtToken
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("expires_at")]
    public DateTime ExpireAt { get; set; }
}