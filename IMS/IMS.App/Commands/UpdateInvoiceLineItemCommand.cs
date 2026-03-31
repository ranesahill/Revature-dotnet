using IMS.App.DTOs;

namespace IMS.App.Commands;

public class UpdateInvoiceLineItemCommand
{
    public int InvoiceId { get; set; }
    public int LineItemId { get; set; }
    public CreateLineItemDto LineItemData { get; set; } = null!;
}
