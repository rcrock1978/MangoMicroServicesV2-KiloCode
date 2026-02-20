using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Models;

/// <summary>
/// Application user entity for authentication
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
