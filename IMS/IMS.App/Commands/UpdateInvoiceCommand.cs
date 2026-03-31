using IMS.App.DTOs;

namespace IMS.App.Commands;

public class UpdateInvoiceCommand
{
    public int InvoiceId { get; set; }
    public UpdateInvoiceDto InvoiceData { get; set; } = null!;
}
