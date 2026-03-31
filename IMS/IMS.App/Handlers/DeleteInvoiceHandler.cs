using IMS.App.Commands;
using IMS.App.Services;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class DeleteInvoiceHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteInvoiceHandler> _logger;

    public DeleteInvoiceHandler(
        IInvoiceRepository invoiceRepo,
        IPaymentRepository paymentRepo,
        ICacheService cacheService,
        ILogger<DeleteInvoiceHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _paymentRepo = paymentRepo;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteInvoiceCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        var payments = await _paymentRepo.GetByInvoiceIdAsync(command.InvoiceId);
        if (payments.Any())
            throw new InvalidOperationException("Cannot delete invoice with existing payments.");

        await _invoiceRepo.DeleteAsync(invoice);
        await _cacheService.RemoveByPrefixAsync("analytics");

        _logger.LogInformation("Invoice {InvoiceId} deleted.", command.InvoiceId);
    }
}
