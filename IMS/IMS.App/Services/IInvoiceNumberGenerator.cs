namespace IMS.App.Services;

public interface IInvoiceNumberGenerator
{
    Task<string> GenerateNextAsync();
}
