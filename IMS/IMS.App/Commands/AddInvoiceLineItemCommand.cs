using IMS.App.DTOs;

namespace IMS.App.Commands;

public class AddInvoiceLineItemCommand
{
    public int InvoiceId { get; set; }
    public CreateLineItemDto LineItemData { get; set; } = null!;
}
