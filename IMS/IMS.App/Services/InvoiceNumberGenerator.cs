using IMS.Core.Interfaces;

namespace IMS.App.Services;

public class InvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceNumberGenerator(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<string> GenerateNextAsync()
    {
        return await _invoiceRepository.GetNextInvoiceNumberAsync();
    }
}
