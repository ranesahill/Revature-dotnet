namespace IMS.App.DTOs;

public class OutstandingDto
{
    public decimal TotalOutstanding { get; set; }
    public int OutstandingInvoiceCount { get; set; }
    public List<InvoiceSummaryDto> OutstandingInvoices { get; set; } = new();
}
