using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Handlers;
using IMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Controllers;

[ApiController]
[Route("api/invoices/{invoiceId}/items")]
[Authorize]
public class InvoiceLineItemsController : ControllerBase
{
    private readonly AddInvoiceLineItemHandler _addHandler;
    private readonly UpdateInvoiceLineItemHandler _updateHandler;
    private readonly DeleteInvoiceLineItemHandler _deleteHandler;
    private readonly IInvoiceLineItemRepository _lineItemRepo;

    public InvoiceLineItemsController(
        AddInvoiceLineItemHandler addHandler,
        UpdateInvoiceLineItemHandler updateHandler,
        DeleteInvoiceLineItemHandler deleteHandler,
        IInvoiceLineItemRepository lineItemRepo)
    {
        _addHandler = addHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _lineItemRepo = lineItemRepo;
    }

    /// <summary>GET /api/invoices/{invoiceId}/items</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceUser,FinanceManager,Admin")]
    public async Task<ActionResult<List<InvoiceLineItemDto>>> GetAll(int invoiceId)
    {
        var items = await _lineItemRepo.GetByInvoiceIdAsync(invoiceId);
        var dtos = items.Select(li => new InvoiceLineItemDto
        {
            LineItemId = li.LineItemId,
            InvoiceId = li.InvoiceId,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            Discount = li.Discount,
            Tax = li.Tax,
            LineTotal = li.LineTotal
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>POST /api/invoices/{invoiceId}/items</summary>
    [HttpPost]
    [Authorize(Roles = "FinanceUser,Admin")]
    public async Task<ActionResult<InvoiceLineItemDto>> Add(int invoiceId, [FromBody] CreateLineItemDto dto)
    {
        try
        {
            var result = await _addHandler.HandleAsync(
                new AddInvoiceLineItemCommand { InvoiceId = invoiceId, LineItemData = dto });
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

    /// <summary>PUT /api/invoices/{invoiceId}/items/{itemId}</summary>
    [HttpPut("{itemId}")]
    [Authorize(Roles = "FinanceUser,Admin")]
    public async Task<ActionResult<InvoiceLineItemDto>> Update(
        int invoiceId, int itemId, [FromBody] CreateLineItemDto dto)
    {
        try
        {
            var result = await _updateHandler.HandleAsync(
                new UpdateInvoiceLineItemCommand
                {
                    InvoiceId = invoiceId,
                    LineItemId = itemId,
                    LineItemData = dto
                });
            return Ok(result);
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

    /// <summary>DELETE /api/invoices/{invoiceId}/items/{itemId}</summary>
    [HttpDelete("{itemId}")]
    [Authorize(Roles = "FinanceUser,Admin")]
    public async Task<ActionResult> Delete(int invoiceId, int itemId)
    {
        try
        {
            await _deleteHandler.HandleAsync(
                new DeleteInvoiceLineItemCommand { InvoiceId = invoiceId, LineItemId = itemId });
            return NoContent();
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
