using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Handlers;
using IMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Controllers;

[ApiController]
[Route("api/invoices/{invoiceId}/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly ApplyPaymentHandler _applyHandler;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        ApplyPaymentHandler applyHandler,
        IPaymentRepository paymentRepo,
        ILogger<PaymentsController> logger)
    {
        _applyHandler = applyHandler;
        _paymentRepo = paymentRepo;
        _logger = logger;
    }

    /// <summary>GET /api/invoices/{invoiceId}/payments</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceUser,FinanceManager,Admin")]
    public async Task<ActionResult<List<PaymentDto>>> GetAll(int invoiceId)
    {
        var payments = await _paymentRepo.GetByInvoiceIdAsync(invoiceId);
        var dtos = payments.Select(p => new PaymentDto
        {
            PaymentId = p.PaymentId,
            InvoiceId = p.InvoiceId,
            PaymentAmount = p.PaymentAmount,
            PaymentDate = p.PaymentDate,
            PaymentMethod = p.PaymentMethod.ToString(),
            ReferenceNumber = p.ReferenceNumber,
            ReceivedDate = p.ReceivedDate
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>POST /api/invoices/{invoiceId}/payments</summary>
    [HttpPost]
    [Authorize(Roles = "FinanceUser,Admin")]
    public async Task<ActionResult<PaymentDto>> ApplyPayment(
        int invoiceId, [FromBody] ApplyPaymentDto dto)
    {
        try
        {
            var result = await _applyHandler.HandleAsync(
                new ApplyPaymentCommand { InvoiceId = invoiceId, PaymentData = dto });
            return CreatedAtAction(nameof(GetAll), new { invoiceId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
