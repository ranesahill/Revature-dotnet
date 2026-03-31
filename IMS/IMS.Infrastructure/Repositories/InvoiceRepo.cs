using IMS.App.Data;
using IMS.Core.Entities;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices.FindAsync(id);
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.LineItems)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);
    }

    public async Task<(List<Invoice> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .OrderByDescending(i => i.CreatedDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Invoice> AddAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Invoice invoice)
    {
        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNumberAsync(string invoiceNumber)
    {
        return await _context.Invoices.AsNoTracking()
            .AnyAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"INV-{year}-";

        var lastInvoice = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastInvoice != null)
        {
            var lastNumberStr = lastInvoice.InvoiceNumber.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out int lastNumber))
                nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    public async Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
    {
        return await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .Where(i => i.Status == status)
            .ToListAsync();
    }

    public async Task<List<Invoice>> GetOverdueInvoicesAsync()
    {
        return await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .Where(i => i.OutstandingBalance > 0 &&
                        i.Status != InvoiceStatus.Paid &&
                        i.Status != InvoiceStatus.Cancelled)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalOutstandingAsync()
    {
        return await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .SumAsync(i => i.OutstandingBalance);
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status != InvoiceStatus.Cancelled &&
                        i.Status != InvoiceStatus.Draft)
            .SumAsync(i => i.GrandTotal);
    }

    public async Task<int> GetInvoiceCountAsync()
    {
        return await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}