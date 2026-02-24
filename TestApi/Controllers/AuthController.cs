using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public record LoginRequest(string Username, string Password);

    /// <summary>
    /// Login using IdentityServer (resource owner password flow).
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        var tokenEndpoint = $"{Request.Scheme}://{Request.Host}/connect/token";

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = "test-client",
            ["client_secret"] = "secret",
            ["grant_type"] = "password",
            ["username"] = request.Username,
            ["password"] = request.Password,
            ["scope"] = "api openid profile"
        });

        var response = await client.PostAsync(tokenEndpoint, form);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, content);
        }

        return Content(content, "application/json", Encoding.UTF8);
    }
}

