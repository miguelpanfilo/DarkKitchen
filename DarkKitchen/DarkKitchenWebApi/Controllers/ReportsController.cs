using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Exceptions;
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
        ValidateDateRange(dateFrom, dateTo);

        var result = _reportService.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }

    [HttpGet("sales")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetSalesReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        var result = _reportService.GetSalesReport(dateFrom, dateTo);
        return Ok(result);
    }

    private static void ValidateDateRange(DateTime? dateFrom, DateTime? dateTo)
    {
        if(dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            throw new ValidationException("Fecha inicio no puede ser posterior a fecha fin");
        }
    }
}
