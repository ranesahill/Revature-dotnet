using IMS.Core.Entities;

namespace IMS.Core.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<List<Payment>> GetByInvoiceIdAsync(int invoiceId);
    Task<Payment> AddAsync(Payment payment);
    Task<decimal> GetTotalPaymentsByInvoiceIdAsync(int invoiceId);
    Task SaveChangesAsync();
}
