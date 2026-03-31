using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class UpdateInvoiceLineItemHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IInvoiceLineItemRepository _lineItemRepo;
    private readonly ILogger<UpdateInvoiceLineItemHandler> _logger;

    public UpdateInvoiceLineItemHandler(
        IInvoiceRepository invoiceRepo,
        IInvoiceLineItemRepository lineItemRepo,
        ILogger<UpdateInvoiceLineItemHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _lineItemRepo = lineItemRepo;
        _logger = logger;
    }

    public async Task<InvoiceLineItemDto> HandleAsync(UpdateInvoiceLineItemCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        if (invoice.Status == Core.Enums.InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot modify items on a paid invoice.");

        var lineItem = await _lineItemRepo.GetByIdAsync(command.LineItemId)
            ?? throw new KeyNotFoundException($"Line item {command.LineItemId} not found.");

        if (lineItem.InvoiceId != command.InvoiceId)
            throw new InvalidOperationException("Line item does not belong to this invoice.");

        var dto = command.LineItemData;
        lineItem.Description = dto.Description;
        lineItem.Quantity = dto.Quantity;
        lineItem.UnitPrice = dto.UnitPrice;
        lineItem.Discount = dto.Discount;
        lineItem.Tax = dto.Tax;
        lineItem.LineTotal = (dto.Quantity * dto.UnitPrice) - dto.Discount + dto.Tax;

        await _lineItemRepo.UpdateAsync(lineItem);

        // Recalculate invoice totals
        invoice.SubTotal = invoice.LineItems.Sum(li => li.LineTotal);
        invoice.GrandTotal = invoice.SubTotal - invoice.DiscountAmount;
        invoice.OutstandingBalance = invoice.GrandTotal - invoice.Payments.Sum(p => p.PaymentAmount);
        await _invoiceRepo.UpdateAsync(invoice);

        _logger.LogInformation("Line item {LineItemId} updated on Invoice {InvoiceId}.",
            command.LineItemId, command.InvoiceId);

        return new InvoiceLineItemDto
        {
            LineItemId = lineItem.LineItemId,
            InvoiceId = lineItem.InvoiceId,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice,
            Discount = lineItem.Discount,
            Tax = lineItem.Tax,
            LineTotal = lineItem.LineTotal
        };
    }
}
