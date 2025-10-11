using API.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService m_AuthService;

    public AuthController(AuthService authService)
    {
        m_AuthService = authService;
    }

    [HttpGet("github")]
    public IActionResult LoginGitHub()
    {
        return Redirect(m_AuthService.GetGitHubAuthorizeUrl());
    }

    [HttpGet("github/callback")]
    public async Task<IActionResult> GitHubCallback([FromQuery] string code)
    {
        var jwtToken = await m_AuthService.GitHubCallback(code);
        if (jwtToken == null) return StatusCode(500);
        return Ok(new { token = jwtToken });
    }
}