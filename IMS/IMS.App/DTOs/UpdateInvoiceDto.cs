using System.ComponentModel.DataAnnotations;

namespace IMS.App.DTOs;

public class UpdateInvoiceDto
{
    public int? CustomerId { get; set; }
    public int? QuoteId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? TaxAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountAmount { get; set; }
}
