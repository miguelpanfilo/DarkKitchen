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
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult GetById(int id)
    {
        var order = _orderFacade.GetById(id);

        if (CurrentUser.Role == UserRolesDto.Client && order.ClientId != CurrentUser.Id)
        {
            return Forbid();
        }

        return Ok(order);
    }

    [HttpGet]
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult GetOrders(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? street,
        [FromQuery] OrderStatus? status)
    {
        if (CurrentUser.Role == UserRolesDto.Client)
        {
            return Ok(_orderFacade.GetClientOrders(CurrentUser.Id, dateFrom, dateTo, status));
        }

        if (!dateFrom.HasValue || !dateTo.HasValue)
        {
            return BadRequest("El rango de fechas es obligatorio");
        }

        return Ok(_orderFacade.GetOrdersByFilters(dateFrom.Value, dateTo.Value, street, status));
    }

    [HttpPut("{id:int}/status")]
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult ChangeStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        switch (request.Estado)
        {
            case "Preparado":
                if (CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return Forbid();
                }

                _orderFacade.Prepare(id);
                break;
            case "Cancelado":
                if (CurrentUser.Role != UserRolesDto.Client && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return Forbid();
                }

                _orderFacade.Cancel(id);
                break;
            case "EnCamino":
                if (CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return Forbid();
                }

                _orderFacade.Dispatch(id);
                break;
            case "Entregado":
                if (CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return Forbid();
                }

                _orderFacade.Deliver(id);
                break;
            case "NoEntregado":
                if (CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return Forbid();
                }

                _orderFacade.MarkNotDelivered(id);
                break;
            default:
                return BadRequest($"Estado '{request.Estado}' no válido");
        }

        var order = _orderFacade.GetById(id);
        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id,
            Estado = order.Status,
            FechaCambioEstado = order.StatusUpdatedAt
        });
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