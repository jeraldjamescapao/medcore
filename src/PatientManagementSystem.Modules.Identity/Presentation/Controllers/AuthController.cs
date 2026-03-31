namespace PatientManagementSystem.Modules.Identity.Presentation.Controllers;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagementSystem.Common.Controllers;
using PatientManagementSystem.Modules.Identity.Application.Abstractions.Authentication;
using PatientManagementSystem.Modules.Identity.Application.Contracts.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest request, CancellationToken ct = default)
    {
        return ToActionResult(await _authService.RegisterAsync(request, ct));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest request, CancellationToken ct = default)
    {
        return ToActionResult(await _authService.LoginAsync(request, ct));
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(CancellationToken ct = default)
    {
        var token = Request.Cookies["refresh_token"] ?? string.Empty;
        return ToActionResult(await _authService.RefreshAsync(token, ct));
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken ct = default)
    {
        var token = Request.Cookies["refresh_token"] ?? string.Empty;
        var result = await _authService.LogoutAsync(token, ct);
        return result.IsSuccess ? NoContent() : ToActionResult(result);
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAllAsync(CancellationToken ct = default)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var result = await _authService.LogoutAllAsync(userId, ct);
        return result.IsSuccess ? NoContent() : ToActionResult(result);
    }
}