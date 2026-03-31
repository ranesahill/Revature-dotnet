using IMS.Core.Entities;
using IMS.Core.Enums;

namespace IMS.Core.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<Invoice?> GetByIdWithDetailsAsync(int id);
    Task<(List<Invoice> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Invoice> AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
    Task DeleteAsync(Invoice invoice);
    Task<bool> ExistsByNumberAsync(string invoiceNumber);
    Task<string> GetNextInvoiceNumberAsync();
    Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
    Task<List<Invoice>> GetOverdueInvoicesAsync();
    Task<decimal> GetTotalOutstandingAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetInvoiceCountAsync();
    Task SaveChangesAsync();
}