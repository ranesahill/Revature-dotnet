using IMS.Core.Entities;

namespace IMS.Core.Interfaces;

public interface IInvoiceLineItemRepository
{
    Task<InvoiceLineItem?> GetByIdAsync(int id);
    Task<List<InvoiceLineItem>> GetByInvoiceIdAsync(int invoiceId);
    Task<InvoiceLineItem> AddAsync(InvoiceLineItem lineItem);
    Task UpdateAsync(InvoiceLineItem lineItem);
    Task DeleteAsync(InvoiceLineItem lineItem);
    Task SaveChangesAsync();
}
