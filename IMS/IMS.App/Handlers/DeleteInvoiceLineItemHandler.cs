using IMS.App.Commands;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class DeleteInvoiceLineItemHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IInvoiceLineItemRepository _lineItemRepo;
    private readonly ILogger<DeleteInvoiceLineItemHandler> _logger;

    public DeleteInvoiceLineItemHandler(
        IInvoiceRepository invoiceRepo,
        IInvoiceLineItemRepository lineItemRepo,
        ILogger<DeleteInvoiceLineItemHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _lineItemRepo = lineItemRepo;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteInvoiceLineItemCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        if (invoice.Status == Core.Enums.InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot delete items from a paid invoice.");

        var lineItem = await _lineItemRepo.GetByIdAsync(command.LineItemId)
            ?? throw new KeyNotFoundException($"Line item {command.LineItemId} not found.");

        if (lineItem.InvoiceId != command.InvoiceId)
            throw new InvalidOperationException("Line item does not belong to this invoice.");

        await _lineItemRepo.DeleteAsync(lineItem);

        // Recalculate invoice totals
        invoice.SubTotal = invoice.LineItems.Where(li => li.LineItemId != command.LineItemId).Sum(li => li.LineTotal);
        invoice.GrandTotal = invoice.SubTotal - invoice.DiscountAmount;
        invoice.OutstandingBalance = invoice.GrandTotal - invoice.Payments.Sum(p => p.PaymentAmount);
        await _invoiceRepo.UpdateAsync(invoice);

        _logger.LogInformation("Line item {LineItemId} deleted from Invoice {InvoiceId}.",
            command.LineItemId, command.InvoiceId);
    }
}
