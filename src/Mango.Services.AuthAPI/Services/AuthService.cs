using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AuthAPI.Services;

/// <summary>
/// Authentication service implementation with JWT token generation
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegistrationRequestDto request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User already exists"
                };
            }

            // Create new user
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assign role
            await AssignRoleAsync(request.Email, request.Role);

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = request.Role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", request.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials"
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "Customer"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user: {Email}", request.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async Task<AuthResponseDto> AssignRoleAsync(string email, string roleName)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            // Create role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Assign role to user
            await _userManager.AddToRoleAsync(user, roleName);

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Role {Role} assigned to user {Email}", roleName, email);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = $"Role {roleName} assigned successfully",
                Token = GenerateJwtToken(user, roles),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = roleName
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user: {Email}", email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred while assigning role"
            };
        }
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345"));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "MangoAuthAPI",
            audience: _configuration["Jwt:Audience"] ?? "MangoClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
