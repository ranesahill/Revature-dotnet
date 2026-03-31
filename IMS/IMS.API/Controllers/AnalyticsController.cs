using IMS.App.Handlers;
using IMS.App.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Controllers;

[ApiController]
[Route("api/invoices/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly GetAgingReportHandler _agingHandler;
    private readonly GetInvoiceAnalyticsHandler _analyticsHandler;
    private readonly GetDsoHandler _dsoHandler;
    private readonly GetOutstandingHandler _outstandingHandler;

    public AnalyticsController(
        GetAgingReportHandler agingHandler,
        GetInvoiceAnalyticsHandler analyticsHandler,
        GetDsoHandler dsoHandler,
        GetOutstandingHandler outstandingHandler)
    {
        _agingHandler = agingHandler;
        _analyticsHandler = analyticsHandler;
        _dsoHandler = dsoHandler;
        _outstandingHandler = outstandingHandler;
    }

    /// <summary>GET /api/invoices/analytics/aging</summary>
    [HttpGet("aging")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<IActionResult> GetAgingReport()
    {
        var result = await _agingHandler.HandleAsync(new GetAgingReportQuery());
        return Ok(result);
    }

    /// <summary>GET /api/invoices/analytics/revenue-summary</summary>
    [HttpGet("revenue-summary")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<IActionResult> GetRevenueSummary()
    {
        var result = await _analyticsHandler.HandleAsync(new GetInvoiceAnalyticsQuery());
        return Ok(result);
    }

    /// <summary>GET /api/invoices/analytics/dso</summary>
    [HttpGet("dso")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<IActionResult> GetDso([FromQuery] int days = 90)
    {
        var result = await _dsoHandler.HandleAsync(new GetDsoQuery { NumberOfDays = days });
        return Ok(result);
    }

    /// <summary>GET /api/invoices/analytics/outstanding</summary>
    [HttpGet("outstanding")]
    [Authorize(Roles = "FinanceManager,Admin")]
    public async Task<IActionResult> GetOutstanding()
    {
        var result = await _outstandingHandler.HandleAsync(new GetOutstandingQuery());
        return Ok(result);
    }
}
