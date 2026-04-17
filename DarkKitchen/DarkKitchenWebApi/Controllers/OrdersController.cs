using Contratos;
using DarkKitchen.Filters;
using Dominio;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController(IOrderFacade orderFacade) : ControllerBase
{
    private readonly IOrderFacade _orderFacade = orderFacade;

    private CurrentUserDto CurrentUser =>
        (CurrentUserDto)HttpContext.Items["CurrentUser"]!;

    [HttpPost]
    [RoleAuthorization(UserRolesDto.Client)]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        var response = _orderFacade.CreateOrder(CurrentUser.Id, request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return Ok(_orderFacade.GetById(id));
    }

    [HttpGet("mine")]
    [RoleAuthorization(UserRolesDto.Client)]
    public IActionResult GetClientOrders(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] OrderStatus? status)
    {
        return Ok(_orderFacade.GetClientOrders(CurrentUser.Id, dateFrom, dateTo, status));
    }

    [HttpGet]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult GetOrdersByFilters(
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo,
        [FromQuery] string? street,
        [FromQuery] OrderStatus? status)
    {
        return Ok(_orderFacade.GetOrdersByFilters(dateFrom, dateTo, street, status));
    }

    [HttpPut("{id:int}/prepare")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Prepare(int id)
    {
        _orderFacade.Prepare(id);
        return NoContent();
    }

    [HttpPut("{id:int}/cancel")]
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Administrative)]
    public IActionResult Cancel(int id)
    {
        _orderFacade.Cancel(id);
        return NoContent();
    }

    [HttpPut("{id:int}/dispatch")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Dispatch(int id)
    {
        _orderFacade.Dispatch(id);
        return NoContent();
    }

    [HttpPut("{id:int}/deliver")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Deliver(int id)
    {
        _orderFacade.Deliver(id);
        return NoContent();
    }

    [HttpPut("{id:int}/not-delivered")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult MarkNotDelivered(int id)
    {
        _orderFacade.MarkNotDelivered(id);
        return NoContent();
    }
}