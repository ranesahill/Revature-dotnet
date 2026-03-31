using System.ComponentModel.DataAnnotations;

namespace IMS.App.DTOs;

public class ApplyPaymentDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be positive.")]
    public decimal PaymentAmount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
}
