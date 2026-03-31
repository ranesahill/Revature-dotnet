using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.Core.Entities;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class AddInvoiceLineItemHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IInvoiceLineItemRepository _lineItemRepo;
    private readonly ILogger<AddInvoiceLineItemHandler> _logger;

    public AddInvoiceLineItemHandler(
        IInvoiceRepository invoiceRepo,
        IInvoiceLineItemRepository lineItemRepo,
        ILogger<AddInvoiceLineItemHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _lineItemRepo = lineItemRepo;
        _logger = logger;
    }

    public async Task<InvoiceLineItemDto> HandleAsync(AddInvoiceLineItemCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        if (invoice.Status == Core.Enums.InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot add items to a paid invoice.");

        var dto = command.LineItemData;
        var lineItem = new InvoiceLineItem
        {
            InvoiceId = command.InvoiceId,
            Description = dto.Description,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            Discount = dto.Discount,
            Tax = dto.Tax,
            LineTotal = (dto.Quantity * dto.UnitPrice) - dto.Discount + dto.Tax
        };

        var created = await _lineItemRepo.AddAsync(lineItem);

        // Recalculate invoice totals
        invoice.SubTotal = invoice.LineItems.Sum(li => li.LineTotal) + lineItem.LineTotal;
        invoice.GrandTotal = invoice.SubTotal - invoice.DiscountAmount;
        invoice.OutstandingBalance = invoice.GrandTotal - invoice.Payments.Sum(p => p.PaymentAmount);
        await _invoiceRepo.UpdateAsync(invoice);

        _logger.LogInformation("Line item added to Invoice {InvoiceId}.", command.InvoiceId);

        return new InvoiceLineItemDto
        {
            LineItemId = created.LineItemId,
            InvoiceId = created.InvoiceId,
            Description = created.Description,
            Quantity = created.Quantity,
            UnitPrice = created.UnitPrice,
            Discount = created.Discount,
            Tax = created.Tax,
            LineTotal = created.LineTotal
        };
    }
}
