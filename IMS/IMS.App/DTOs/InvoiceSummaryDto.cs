namespace IMS.App.DTOs;

public class InvoiceSummaryDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public decimal OutstandingBalance { get; set; }
    public bool IsOverdue { get; set; }
}
