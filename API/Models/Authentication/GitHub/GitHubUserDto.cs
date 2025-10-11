using System.Text.Json.Serialization;

namespace API.Models.Authentication;

public class GitHubUserDto
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = null!;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
}