using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Services;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class UpdateInvoiceHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UpdateInvoiceHandler> _logger;

    public UpdateInvoiceHandler(
        IInvoiceRepository invoiceRepo,
        ICacheService cacheService,
        ILogger<UpdateInvoiceHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<InvoiceDto> HandleAsync(UpdateInvoiceCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        if (invoice.Status == Core.Enums.InvoiceStatus.Paid)
            throw new InvalidOperationException("Paid invoices cannot be modified.");

        var dto = command.InvoiceData;

        if (dto.CustomerId.HasValue)
            invoice.CustomerId = dto.CustomerId.Value;

        if (dto.QuoteId.HasValue)
            invoice.QuoteId = dto.QuoteId;

        if (dto.InvoiceDate.HasValue)
            invoice.InvoiceDate = dto.InvoiceDate.Value;

        if (dto.DueDate.HasValue)
            invoice.DueDate = dto.DueDate.Value;

        if (invoice.DueDate <= invoice.InvoiceDate)
            throw new InvalidOperationException("DueDate must be greater than InvoiceDate.");

        if (dto.TaxAmount.HasValue)
            invoice.TaxAmount = dto.TaxAmount.Value;

        if (dto.DiscountAmount.HasValue)
            invoice.DiscountAmount = dto.DiscountAmount.Value;

        // Recalculate
        invoice.SubTotal = invoice.LineItems.Sum(li => li.LineTotal);
        invoice.GrandTotal = invoice.SubTotal - invoice.DiscountAmount;
        invoice.OutstandingBalance = invoice.GrandTotal - invoice.Payments.Sum(p => p.PaymentAmount);

        await _invoiceRepo.UpdateAsync(invoice);
        await _cacheService.RemoveByPrefixAsync("analytics");

        _logger.LogInformation("Invoice {InvoiceId} updated.", command.InvoiceId);

        return new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.CustomerName,
            QuoteId = invoice.QuoteId,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToString(),
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            GrandTotal = invoice.GrandTotal,
            OutstandingBalance = invoice.OutstandingBalance,
            CreatedDate = invoice.CreatedDate
        };
    }
}
