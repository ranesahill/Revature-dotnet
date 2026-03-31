using IMS.Core.Enums;

namespace IMS.App.Commands;

public class ChangeInvoiceStatusCommand
{
    public int InvoiceId { get; set; }
    public InvoiceStatus NewStatus { get; set; }
}
