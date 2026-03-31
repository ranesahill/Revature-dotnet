using IMS.App.DTOs;

namespace IMS.App.Commands;

public class ApplyPaymentCommand
{
    public int InvoiceId { get; set; }
    public ApplyPaymentDto PaymentData { get; set; } = null!;
}
