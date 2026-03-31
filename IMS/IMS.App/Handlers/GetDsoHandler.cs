using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.App.Services;
using IMS.Core.Interfaces;

namespace IMS.App.Handlers;

public class GetDsoHandler
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly ICacheService _cacheService;

    public GetDsoHandler(IInvoiceRepository invoiceRepo, ICacheService cacheService)
    {
        _invoiceRepo = invoiceRepo;
        _cacheService = cacheService;
    }

    public async Task<DsoDto> HandleAsync(GetDsoQuery query)
    {
        const string cacheKey = "analytics:dso";

        var cached = await _cacheService.GetAsync<DsoDto>(cacheKey);
        if (cached != null) return cached;

        var totalOutstanding = await _invoiceRepo.GetTotalOutstandingAsync();
        var totalCreditSales = await _invoiceRepo.GetTotalRevenueAsync();

        decimal dso = 0;
        if (totalCreditSales > 0)
            dso = (totalOutstanding / totalCreditSales) * query.NumberOfDays;

        var result = new DsoDto
        {
            DaysSalesOutstanding = Math.Round(dso, 2),
            TotalOutstanding = totalOutstanding,
            TotalCreditSales = totalCreditSales,
            NumberOfDays = query.NumberOfDays
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}
