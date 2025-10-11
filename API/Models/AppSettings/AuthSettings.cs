namespace API.Models;

public class AuthSettings
{
    public class GitHubSettings
    {
        public string ClientIdKey { get; set; } = null!;
        public string ClientSecretKey { get; set; } = null!;
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpirationMinutes { get; set; }
    }

    public GitHubSettings GitHub { get; set; } = null!;
    public JwtSettings Jwt { get; set; } = null!;
}