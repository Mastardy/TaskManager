using System.Net.Http.Headers;
using API.Models;
using API.Models.Authentication;
using API.Utils;
using Microsoft.Extensions.Options;

namespace API.Services.Authentication;

public class AuthService
{
    private readonly AuthSettings.GitHubSettings m_GitHubSettings;
    private readonly HttpClient m_HttpClient;
    private readonly TokenService m_TokenService;

    public AuthService(IOptions<AuthSettings> authSettings, TokenService tokenService)
    {
        m_GitHubSettings = authSettings.Value.GitHub;
        m_HttpClient = new HttpClient();
        m_TokenService = tokenService;
    }

    public string GetGitHubAuthorizeUrl()
    {
        var clientId = EnvHelper.Get(m_GitHubSettings.ClientIdKey);
        var scopes = "read:user+user:email";

        return $"https://github.com/login/oauth/authorize?client_id={clientId}&scope={scopes}";
    }

    public async Task<string?> GitHubCallback(string code)
    {
        var token = await GetGitHubAccessToken(code);

        var userData = await GetGitHubUserData(token);
        if (userData == null) return null;
        else if (string.IsNullOrEmpty(userData.Email)) return null;

        var username = userData.Login;

        return m_TokenService.GenerateToken(userData.Id.ToString(), username);
    }


    private async Task<string> GetGitHubAccessToken(string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", EnvHelper.Get(m_GitHubSettings.ClientIdKey)),
            new KeyValuePair<string, string>("client_secret", EnvHelper.Get(m_GitHubSettings.ClientSecretKey)),
            new KeyValuePair<string, string>("code", code)
        });
        request.Content = content;

        var response = await m_HttpClient.SendAsync(request);
        var json = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>();
        if (json == null) throw new NullReferenceException("GitHubTokenResponse json is Null!");

        return json.AccessToken;
    }

    private async Task<GitHubUserDto?> GetGitHubUserData(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.UserAgent.ParseAdd("Mastardy Boards 1.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var response = await m_HttpClient.SendAsync(request);

        var user = await response.Content.ReadFromJsonAsync<GitHubUserDto>();
        if (user == null) return null;

        if (string.IsNullOrEmpty(user.Email))
        {
            var email = await GetGitHubUserEmail(token);
            if (email != null) user.Email = email;
        }

        return user;
    }

    private async Task<string?> GetGitHubUserEmail(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.UserAgent.ParseAdd("Mastardy Boards 1.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        var response = await m_HttpClient.SendAsync(request);

        var user = await response.Content.ReadFromJsonAsync<List<GitHubUserEmailDto>>();
        if (user == null) return null;

        foreach (var userEmail in user)
        {
            if (userEmail.Primary) return userEmail.Email;
        }

        return string.Empty;
    }
}