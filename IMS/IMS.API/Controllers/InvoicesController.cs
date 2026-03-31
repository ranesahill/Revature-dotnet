using IMS.App.Commands;
using IMS.App.DTOs;
using IMS.App.Handlers;
using IMS.App.Queries;
using IMS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly CreateInvoiceHandler _createHandler;
    private readonly UpdateInvoiceHandler _updateHandler;
    private readonly DeleteInvoiceHandler _deleteHandler;
    private readonly ChangeInvoiceStatusHandler _statusHandler;
    private readonly GetInvoiceByIdHandler _getByIdHandler;
    private readonly GetAllInvoicesHandler _getAllHandler;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        CreateInvoiceHandler createHandler,
        UpdateInvoiceHandler updateHandler,
        DeleteInvoiceHandler deleteHandler,
        ChangeInvoiceStatusHandler statusHandler,
        GetInvoiceByIdHandler getByIdHandler,
        GetAllInvoicesHandler getAllHandler,
        ILogger<InvoicesController> logger)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _statusHandler = statusHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
        _logger = logger;
    }

    /// <summary>GET /api/invoices</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceUser,FinanceManager,Admin")]
    public async Task<ActionResult<PaginatedResultDto<InvoiceSummaryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetAllInvoicesQuery { Page = page, PageSize = pageSize });
        return Ok(result);
    }

    /// <summary>GET /api/invoices/{id}</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "FinanceUser,FinanceManager,Admin")]
    public async Task<ActionResult<InvoiceDto>> GetById(int id)
    {
        try
        {
            var result = await _getByIdHandler.HandleAsync(
                new GetInvoiceByIdQuery { InvoiceId = id });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>POST /api/invoices</summary>
    [HttpPost]
    [Authorize(Roles = "FinanceUser,Admin")]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var result = await _createHandler.HandleAsync(
                new CreateInvoiceCommand { InvoiceData = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.InvoiceId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>PUT /api/invoices/{id}</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<ActionResult<InvoiceDto>> Update(int id, [FromBody] UpdateInvoiceDto dto)
    {
        try
        {
            var result = await _updateHandler.HandleAsync(
                new UpdateInvoiceCommand { InvoiceId = id, InvoiceData = dto });
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

    /// <summary>DELETE /api/invoices/{id}</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _deleteHandler.HandleAsync(new DeleteInvoiceCommand { InvoiceId = id });
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

    /// <summary>PATCH /api/invoices/{id}/status</summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<ActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
    {
        try
        {
            if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var status))
                return BadRequest(new { message = $"Invalid status: {dto.Status}" });

            await _statusHandler.HandleAsync(
                new ChangeInvoiceStatusCommand { InvoiceId = id, NewStatus = status });
            return Ok(new { message = $"Status changed to {status}." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public class ChangeStatusDto
{
    public string Status { get; set; } = string.Empty;
}
