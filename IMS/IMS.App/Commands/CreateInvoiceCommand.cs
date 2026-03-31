using IMS.App.DTOs;

namespace IMS.App.Commands;

public class CreateInvoiceCommand
{
    public CreateInvoiceDto InvoiceData { get; set; } = null!;
}