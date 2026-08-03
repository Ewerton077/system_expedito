using System.ComponentModel.DataAnnotations;

namespace SantoExpedito.Models;

public enum UserRole
{
    Admin,
    SuperAdmin
}

public class AdminUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Admin;
}
