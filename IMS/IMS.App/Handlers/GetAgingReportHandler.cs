using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.App.Services;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.App.Handlers;

public class GetAgingReportHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetAgingReportHandler> _logger;

    public GetAgingReportHandler(
        IInvoiceRepository invoiceRepo,
        ICacheService cacheService,
        ILogger<GetAgingReportHandler> logger)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<AgingReportDto> HandleAsync(GetAgingReportQuery query)
    {
        const string cacheKey = "analytics:aging";

        var cached = await _cacheService.GetAsync<AgingReportDto>(cacheKey);
        if (cached != null) return cached;

        var overdueInvoices = await _invoiceRepo.GetOverdueInvoicesAsync();
        var now = DateTime.UtcNow;

        var report = new AgingReportDto();

        foreach (var invoice in overdueInvoices)
        {
            var daysOverdue = (now - invoice.DueDate).Days;

            if (daysOverdue <= 0)
                report.Current += invoice.OutstandingBalance;
            else if (daysOverdue <= 30)
                report.OneToThirtyDays += invoice.OutstandingBalance;
            else if (daysOverdue <= 60)
                report.ThirtyOneToSixtyDays += invoice.OutstandingBalance;
            else
                report.SixtyPlusDays += invoice.OutstandingBalance;
        }

        report.TotalOutstanding = report.Current + report.OneToThirtyDays +
                                   report.ThirtyOneToSixtyDays + report.SixtyPlusDays;
        report.TotalOverdueInvoices = overdueInvoices.Count;

        await _cacheService.SetAsync(cacheKey, report, TimeSpan.FromMinutes(5));

        return report;
    }
}
