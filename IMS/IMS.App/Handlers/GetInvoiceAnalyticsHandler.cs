using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.App.Services;
using IMS.Core.Enums;
using IMS.Core.Interfaces;

namespace IMS.App.Handlers;

public class GetInvoiceAnalyticsHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;

    public GetInvoiceAnalyticsHandler(IInvoiceRepository invoiceRepo, ICacheService cacheService)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
    }

    public async Task<RevenueSummaryDto> HandleAsync(GetInvoiceAnalyticsQuery query)
    {
        const string cacheKey = "analytics:revenue";

        var cached = await _cacheService.GetAsync<RevenueSummaryDto>(cacheKey);
        if (cached != null) return cached;

        var totalRevenue = await _invoiceRepo.GetTotalRevenueAsync();
        var totalOutstanding = await _invoiceRepo.GetTotalOutstandingAsync();
        var totalInvoices = await _invoiceRepo.GetInvoiceCountAsync();

        var paidInvoices = (await _invoiceRepo.GetInvoicesByStatusAsync(InvoiceStatus.Paid)).Count;
        var overdueInvoices = (await _invoiceRepo.GetOverdueInvoicesAsync()).Count;
        var partiallyPaid = (await _invoiceRepo.GetInvoicesByStatusAsync(InvoiceStatus.PartiallyPaid)).Count;

        var result = new RevenueSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalPaid = totalRevenue - totalOutstanding,
            TotalOutstanding = totalOutstanding,
            TotalInvoices = totalInvoices,
            PaidInvoices = paidInvoices,
            OverdueInvoices = overdueInvoices,
            PartiallyPaidInvoices = partiallyPaid
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}
