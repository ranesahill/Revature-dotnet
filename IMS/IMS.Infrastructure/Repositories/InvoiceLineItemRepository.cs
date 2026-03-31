using IMS.App.Data;
using IMS.Core.Entities;
using IMS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Repositories;

public class InvoiceLineItemRepository : IInvoiceLineItemRepository
{
    private readonly AppDbContext _context;

    public InvoiceLineItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceLineItem?> GetByIdAsync(int id)
    {
        return await _context.InvoiceLineItems.FindAsync(id);
    }

    public async Task<List<InvoiceLineItem>> GetByInvoiceIdAsync(int invoiceId)
    {
        return await _context.InvoiceLineItems
            .AsNoTracking()
            .Where(li => li.InvoiceId == invoiceId)
            .ToListAsync();
    }

    public async Task<InvoiceLineItem> AddAsync(InvoiceLineItem lineItem)
    {
        _context.InvoiceLineItems.Add(lineItem);
        await _context.SaveChangesAsync();
        return lineItem;
    }

    public async Task UpdateAsync(InvoiceLineItem lineItem)
    {
        _context.InvoiceLineItems.Update(lineItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(InvoiceLineItem lineItem)
    {
        _context.InvoiceLineItems.Remove(lineItem);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
