using Newtonsoft.Json;

namespace API.Models.Authentication;

public class GitHubUserEmailDto
{
    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    [JsonProperty("primary")]
    public bool Primary { get; set; }
}