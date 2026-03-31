using System.ComponentModel.DataAnnotations;

namespace IMS.App.DTOs;

public class CreateInvoiceDto
{
    [Required]
    public int CustomerId { get; set; }

    public int? QuoteId { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    public List<CreateLineItemDto> LineItems { get; set; } = new();
}

public class CreateLineItemDto
{
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Tax { get; set; }
}
