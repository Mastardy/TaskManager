using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using API.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Authentication;

public class TokenService
{
    private readonly AuthSettings.JwtSettings m_JwtSettings;

    public TokenService(IOptions<AuthSettings> authSettings)
    {
        m_JwtSettings = authSettings.Value.Jwt;
    }

    public string GenerateToken(string id, string username)
    {
        var secret = EnvHelper.Get(m_JwtSettings.SecretKey);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, id)
        };

        var token = new JwtSecurityToken(
            issuer: m_JwtSettings.Issuer,
            audience: m_JwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(m_JwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}