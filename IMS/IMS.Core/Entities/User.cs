using System.ComponentModel.DataAnnotations;
using IMS.Core.Enums;

namespace IMS.Core.Entities;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
