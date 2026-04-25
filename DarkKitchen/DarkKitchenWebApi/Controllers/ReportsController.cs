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
    public IActionResult GetTopProductos(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
        {
            return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin");
        }

        var result = _orderFacade.GetTopProductos(fechaDesde, fechaHasta);
        return Ok(result);
    }

    [HttpGet("sales")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetReporteVentas(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
        {
            return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin");
        }

        var result = _orderFacade.GetReporteVentas(fechaDesde, fechaHasta);
        return Ok(result);
    }
}
