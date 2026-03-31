using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Services;
using IMS.Core.Entities;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class CreateInvoiceHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IInvoiceNumberGenerator _numberGenerator;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateInvoiceHandler> _logger;

    public CreateInvoiceHandler(
        IInvoiceRepository invoiceRepo,
        IInvoiceNumberGenerator numberGenerator,
        ICacheService cacheService,
        ILogger<CreateInvoiceHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _numberGenerator = numberGenerator;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<InvoiceDto> HandleAsync(CreateInvoiceCommand command)
    {
        var dto = command.InvoiceData;

        if (dto.DueDate <= dto.InvoiceDate)
            throw new InvalidOperationException("DueDate must be greater than InvoiceDate.");

        var invoiceNumber = await _numberGenerator.GenerateNextAsync();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = dto.CustomerId,
            QuoteId = dto.QuoteId,
            InvoiceDate = dto.InvoiceDate,
            DueDate = dto.DueDate,
            Status = InvoiceStatus.Draft,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            CreatedDate = DateTime.UtcNow
        };

        // Add line items and calculate
        foreach (var item in dto.LineItems)
        {
            var lineItem = new InvoiceLineItem
            {
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Tax = item.Tax,
                LineTotal = (item.Quantity * item.UnitPrice) - item.Discount + item.Tax
            };
            invoice.LineItems.Add(lineItem);
        }

        // Calculate totals
        invoice.SubTotal = invoice.LineItems.Sum(li => li.LineTotal);
        invoice.GrandTotal = invoice.SubTotal - invoice.DiscountAmount;
        invoice.OutstandingBalance = invoice.GrandTotal;

        var created = await _invoiceRepo.AddAsync(invoice);

        // Invalidate analytics cache
        await _cacheService.RemoveByPrefixAsync("analytics");

        _logger.LogInformation("Invoice {InvoiceNumber} created successfully.", invoiceNumber);

        return MapToDto(created);
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
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
            CreatedDate = invoice.CreatedDate,
            LineItems = invoice.LineItems.Select(li => new InvoiceLineItemDto
            {
                LineItemId = li.LineItemId,
                InvoiceId = li.InvoiceId,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                Discount = li.Discount,
                Tax = li.Tax,
                LineTotal = li.LineTotal
            }).ToList(),
            Payments = invoice.Payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                PaymentAmount = p.PaymentAmount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                ReferenceNumber = p.ReferenceNumber,
                ReceivedDate = p.ReceivedDate
            }).ToList()
        };
    }
}