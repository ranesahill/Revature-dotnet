using IMS.App.DTOs;
using IMS.App.Queries;
using IMS.Core.Enums;
using IMS.Core.Interfaces;

namespace IMS.App.Handlers;

public class GetAllInvoicesHandler
{
    private readonly IInvoiceRepository _invoiceRepo;

    public GetAllInvoicesHandler(IInvoiceRepository invoiceRepo)
    {
        _invoiceRepo = invoiceRepo;
    }

    public async Task<PaginatedResultDto<InvoiceSummaryDto>> HandleAsync(GetAllInvoicesQuery query)
    {
        var (items, totalCount) = await _invoiceRepo.GetAllAsync(query.Page, query.PageSize);

        var dtos = items.Select(i => new InvoiceSummaryDto
        {
            InvoiceId = i.InvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.Customer?.CustomerName,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            Status = i.Status.ToString(),
            GrandTotal = i.GrandTotal,
            OutstandingBalance = i.OutstandingBalance,
            IsOverdue = i.DueDate < DateTime.UtcNow &&
                        i.Status != InvoiceStatus.Paid &&
                        i.Status != InvoiceStatus.Cancelled
        }).ToList();

        return new PaginatedResultDto<InvoiceSummaryDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}
