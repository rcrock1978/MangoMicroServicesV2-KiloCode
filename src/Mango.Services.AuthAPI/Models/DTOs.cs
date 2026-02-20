using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTOs;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Registration request DTO
/// </summary>
public class RegistrationRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public string Role { get; set; } = "Customer";
}

/// <summary>
/// User DTO for responses
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Auth response DTO
/// </summary>
public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}
