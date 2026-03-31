using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.Core.Interfaces;

namespace IMS.App.Handlers;

public class GetInvoiceByIdHandler
{
    private readonly IInvoiceRepository _invoiceRepo;

    public GetInvoiceByIdHandler(IInvoiceRepository invoiceRepo)
    {
        _invoiceRepo = invoiceRepo;
    }

    public async Task<InvoiceDto> HandleAsync(GetInvoiceByIdQuery query)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(query.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {query.InvoiceId} not found.");

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