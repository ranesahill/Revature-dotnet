using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Entities;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    // Navigation
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
