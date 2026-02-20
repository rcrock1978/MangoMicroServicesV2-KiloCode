using Mango.Services.AuthAPI.Models.DTOs;

namespace Mango.Services.AuthAPI.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegistrationRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> AssignRoleAsync(string email, string roleName);
}
