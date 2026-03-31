using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.App.Services;
using IMS.Core.Enums;
using IMS.Core.Interfaces;

namespace IMS.App.Handlers;

public class GetOutstandingHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;

    public GetOutstandingHandler(IInvoiceRepository invoiceRepo, ICacheService cacheService)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
    }

    public async Task<OutstandingDto> HandleAsync(GetOutstandingQuery query)
    {
        const string cacheKey = "analytics:outstanding";

        var cached = await _cacheService.GetAsync<OutstandingDto>(cacheKey);
        if (cached != null) return cached;

        var overdue = await _invoiceRepo.GetOverdueInvoicesAsync();

        var result = new OutstandingDto
        {
            TotalOutstanding = await _invoiceRepo.GetTotalOutstandingAsync(),
            OutstandingInvoiceCount = overdue.Count,
            OutstandingInvoices = overdue.Select(i => new InvoiceSummaryDto
            {
                InvoiceId = i.InvoiceId,
                InvoiceNumber = i.InvoiceNumber,
                CustomerName = i.Customer?.CustomerName,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                Status = i.Status.ToString(),
                GrandTotal = i.GrandTotal,
                OutstandingBalance = i.OutstandingBalance,
                IsOverdue = i.DueDate < DateTime.UtcNow
            }).ToList()
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}
