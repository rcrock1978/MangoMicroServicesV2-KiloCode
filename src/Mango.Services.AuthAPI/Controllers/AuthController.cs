using Mango.Services.AuthAPI.Models.DTOs;
using Mango.Services.AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

/// <summary>
/// Authentication API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegistrationRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<ActionResult<AuthResponseDto>> AssignRole([FromBody] AssignRoleDto request)
    {
        var result = await _authService.AssignRoleAsync(request.Email, request.Role);
        return Ok(result);
    }
}

/// <summary>
/// DTO for assigning role
/// </summary>
public class AssignRoleDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
