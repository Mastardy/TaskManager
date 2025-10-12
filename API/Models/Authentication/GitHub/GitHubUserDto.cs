using Newtonsoft.Json;

namespace API.Models.Authentication;

public class GitHubUserDto
{
    [JsonProperty("login")]
    public string Login { get; set; } = null!;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; } = null!;

    [JsonProperty("email")]
    public string Email { get; set; } = null!;
}