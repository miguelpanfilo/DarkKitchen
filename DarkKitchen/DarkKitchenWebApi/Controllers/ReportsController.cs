using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Reports;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    [HttpGet("top-products")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetTopProducts(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = _reportService.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }

    [HttpGet("sales")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetSalesReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = _reportService.GetSalesReport(dateFrom, dateTo);
        return Ok(result);
    }
}
