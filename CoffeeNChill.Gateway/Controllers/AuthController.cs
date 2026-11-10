using Microsoft.AspNetCore.Mvc;
using CoffeeNChill.Gateway.Services;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/users")]
public class AuthController : ControllerBase
{
    private readonly FunctionsProxyService _proxy;

    public AuthController(FunctionsProxyService proxy)
    {
        _proxy = proxy;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] JsonElement request)
    {
        var content = new StringContent(
            request.GetRawText(),
            Encoding.UTF8,
            "application/json");

        var response = await _proxy.ForwardRequestAsync(
            HttpMethod.Post,
            "api/users/register",
            content);

        var body = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] JsonElement request)
    {
        var content = new StringContent(
            request.GetRawText(),
            Encoding.UTF8,
            "application/json");

        var response = await _proxy.ForwardRequestAsync(
            HttpMethod.Post,
            "api/users/login",
            content);

        var body = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfile(string userId)
    {
        var response = await _proxy.ForwardRequestAsync(
            HttpMethod.Get,
            $"api/users/{userId}",
            null);

        var body = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }
}