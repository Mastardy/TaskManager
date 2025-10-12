using Newtonsoft.Json;

namespace API.Models.Authentication;

public class GitHubTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("scope")]
    public string Scope { get; set; } = null!;

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = null!;
}