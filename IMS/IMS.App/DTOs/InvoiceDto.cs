using IMS.Core.Enums;

namespace IMS.App.DTOs;

public class InvoiceDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? QuoteId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal OutstandingBalance { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}