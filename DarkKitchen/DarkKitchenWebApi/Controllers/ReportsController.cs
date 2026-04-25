using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Orders;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController(IOrderFacade orderFacade) : ControllerBase
{
    private readonly IOrderFacade _orderFacade = orderFacade;

    [HttpGet("top")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetTopProducts(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            return BadRequest("Start date cannot be later than end date");
        }

        var result = _orderFacade.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }

    [HttpGet("sales")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetSalesReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            return BadRequest("Start date cannot be later than end date");
        }

        var result = _orderFacade.GetSalesReport(dateFrom, dateTo);
        return Ok(result);
    }
}
