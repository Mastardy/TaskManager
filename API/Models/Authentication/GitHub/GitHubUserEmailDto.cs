using System.Text.Json.Serialization;

namespace API.Models.Authentication;

public class GitHubUserEmailDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}