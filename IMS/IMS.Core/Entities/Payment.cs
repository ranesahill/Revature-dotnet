using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IMS.Core.Enums;

namespace IMS.Core.Entities;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    public int InvoiceId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be positive.")]
    public decimal PaymentAmount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("InvoiceId")]
    public Invoice? Invoice { get; set; }
}
