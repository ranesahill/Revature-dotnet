using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Services;
using IMS.Core.Entities;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class ApplyPaymentHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ApplyPaymentHandler> _logger;

    public ApplyPaymentHandler(
        IInvoiceRepository invoiceRepo,
        IPaymentRepository paymentRepo,
        ICacheService cacheService,
        ILogger<ApplyPaymentHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _paymentRepo = paymentRepo;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PaymentDto> HandleAsync(ApplyPaymentCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdWithDetailsAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cannot apply payment to a cancelled invoice.");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Invoice is already fully paid.");

        var dto = command.PaymentData;

        if (dto.PaymentAmount <= 0)
            throw new InvalidOperationException("Payment amount must be positive.");

        if (dto.PaymentAmount > invoice.OutstandingBalance)
            throw new InvalidOperationException(
                $"Payment amount {dto.PaymentAmount} exceeds outstanding balance {invoice.OutstandingBalance}.");

        if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
            throw new InvalidOperationException($"Invalid payment method: {dto.PaymentMethod}");

        var payment = new Payment
        {
            InvoiceId = command.InvoiceId,
            PaymentAmount = dto.PaymentAmount,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = paymentMethod,
            ReferenceNumber = dto.ReferenceNumber,
            ReceivedDate = DateTime.UtcNow
        };

        // Atomic: add payment + update invoice
        var created = await _paymentRepo.AddAsync(payment);

        invoice.OutstandingBalance -= dto.PaymentAmount;

        if (invoice.OutstandingBalance == 0)
            invoice.Status = InvoiceStatus.Paid;
        else
            invoice.Status = InvoiceStatus.PartiallyPaid;

        await _invoiceRepo.UpdateAsync(invoice);

        // Invalidate analytics cache
        await _cacheService.RemoveByPrefixAsync("analytics");

        _logger.LogInformation(
            "Payment of {Amount} applied to Invoice {InvoiceId}. New balance: {Balance}. Status: {Status}",
            dto.PaymentAmount, command.InvoiceId, invoice.OutstandingBalance, invoice.Status);

        return new PaymentDto
        {
            PaymentId = created.PaymentId,
            InvoiceId = created.InvoiceId,
            PaymentAmount = created.PaymentAmount,
            PaymentDate = created.PaymentDate,
            PaymentMethod = created.PaymentMethod.ToString(),
            ReferenceNumber = created.ReferenceNumber,
            ReceivedDate = created.ReceivedDate
        };
    }
}
