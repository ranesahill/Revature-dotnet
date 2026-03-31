using IMS.App.Commands;
using IMS.App.Services;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class ChangeInvoiceStatusHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ChangeInvoiceStatusHandler> _logger;

    public ChangeInvoiceStatusHandler(
        IInvoiceRepository invoiceRepo,
        ICacheService cacheService,
        ILogger<ChangeInvoiceStatusHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task HandleAsync(ChangeInvoiceStatusCommand command)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(command.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {command.InvoiceId} not found.");

        var oldStatus = invoice.Status;
        invoice.Status = command.NewStatus;

        await _invoiceRepo.UpdateAsync(invoice);
        await _cacheService.RemoveByPrefixAsync("analytics");

        _logger.LogInformation(
            "Invoice {InvoiceId} status changed from {OldStatus} to {NewStatus}.",
            command.InvoiceId, oldStatus, command.NewStatus);
    }
}
