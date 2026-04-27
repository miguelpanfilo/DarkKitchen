using Contratos.Orders;
using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Orders;
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

        if(CurrentUser.Role == UserRolesDto.Client && order.Client.Id != CurrentUser.Id)
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
        [FromQuery] string? status)
    {
        if (!TryParseStatus(status, out var parsedStatus, out var errorMessage))
        {
            return BadRequest(errorMessage);
        }

        if (CurrentUser.Role == UserRolesDto.Client)
        {
            var clientOrders = _orderFacade.GetClientOrders(CurrentUser.Id, dateFrom, dateTo, parsedStatus);
            return Ok(clientOrders);
        }

        if (!HasValidDateRange(dateFrom, dateTo))
        {
            return BadRequest("El rango de fechas es obligatorio");
        }

        var orders = _orderFacade.GetOrdersByFilters(dateFrom!.Value, dateTo!.Value, street, parsedStatus);
        return Ok(orders);
    }

    private static bool TryParseStatus(
        string? status,
        out OrderStatusDto? parsedStatus,
        out string? errorMessage)
    {
        parsedStatus = null;
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        if (Enum.TryParse<OrderStatusDto>(status, true, out var tempStatus))
        {
            parsedStatus = tempStatus;
            return true;
        }

        errorMessage = "El estado ingresado no es válido";
        return false;
    }

    private static bool HasValidDateRange(DateTime? dateFrom, DateTime? dateTo)
    {
        return dateFrom.HasValue && dateTo.HasValue;
    }

    [HttpPut("{id:int}/prepare")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Prepare(int id)
    {
        _orderFacade.Prepare(id);

        var order = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id, Estado = order.Status, FechaCambioEstado = order.StatusUpdatedAt
        });
    }

    [HttpPut("{id:int}/cancel")]
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Administrative)]
    public IActionResult Cancel(int id)
    {
        if(CurrentUser.Role == UserRolesDto.Client)
        {
            var order = _orderFacade.GetById(id);
            if(order.Client.Id != CurrentUser.Id)
            {
                return ForbiddenResponse();
            }
        }

        _orderFacade.Cancel(id);

        var updated = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = updated.Id, Estado = updated.Status, FechaCambioEstado = updated.StatusUpdatedAt
        });
    }

    [HttpPut("{id:int}/dispatch")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Dispatch(int id)
    {
        _orderFacade.Dispatch(id);

        var order = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id, Estado = order.Status, FechaCambioEstado = order.StatusUpdatedAt
        });
    }

    [HttpPut("{id:int}/deliver")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult Deliver(int id)
    {
        _orderFacade.Deliver(id);

        var order = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id, Estado = order.Status, FechaCambioEstado = order.StatusUpdatedAt
        });
    }

    [HttpPut("{id:int}/not-delivered")]
    [RoleAuthorization(UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult MarkNotDelivered(int id)
    {
        _orderFacade.MarkNotDelivered(id);

        var order = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id, Estado = order.Status, FechaCambioEstado = order.StatusUpdatedAt
        });
    }

    private IActionResult ForbiddenResponse()
    {
        return StatusCode(StatusCodes.Status403Forbidden,
            new ResponseDto
            {
                ExecutionSuccessful = false, Message = "El usuario no tiene permisos para ejecutar esta acción"
            });
    }
}

/*
  [HttpPut("{id:int}/status")]
    [RoleAuthorization(UserRolesDto.Client, UserRolesDto.Preparer, UserRolesDto.Administrative)]
    public IActionResult ChangeStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        OrderStatusDto status;

        try
        {
            status = ParseStatus(request.Estado);
        }
        catch(ValidationException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new ResponseDto { ExecutionSuccessful = false, Message = ex.Message });
        }

        switch(status)
        {
            case OrderStatusDto.Prepared:
                if(CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return ForbiddenResponse();
                }

                _orderFacade.Prepare(id);
                break;

            case OrderStatusDto.Cancelled:
                if(CurrentUser.Role != UserRolesDto.Client && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return ForbiddenResponse();
                }

                if(CurrentUser.Role == UserRolesDto.Client)
                {
                    var orderToCancel = _orderFacade.GetById(id);
                    if(orderToCancel.Client.Id != CurrentUser.Id)
                    {
                        return ForbiddenResponse();
                    }
                }

                _orderFacade.Cancel(id);
                break;

            case OrderStatusDto.OnTheWay:
                if(CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return ForbiddenResponse();
                }

                _orderFacade.Dispatch(id);
                break;

            case OrderStatusDto.Delivered:
                if(CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return ForbiddenResponse();
                }

                _orderFacade.Deliver(id);
                break;

            case OrderStatusDto.NotDelivered:
                if(CurrentUser.Role != UserRolesDto.Preparer && CurrentUser.Role != UserRolesDto.Administrative)
                {
                    return ForbiddenResponse();
                }

                _orderFacade.MarkNotDelivered(id);
                break;

            default:
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseDto { ExecutionSuccessful = false, Message = $"Estado '{request.Estado}' no válido" });
        }

        var order = _orderFacade.GetById(id);

        return Ok(new OrderStatusResponse
        {
            PedidoId = order.Id, Estado = order.Status, FechaCambioEstado = order.StatusUpdatedAt
        });
    }
  private static string NormalizeStatus(string status)
    {
        if(string.IsNullOrWhiteSpace(status))
        {
            throw new ValidationException("El estado es obligatorio");
        }

        return status
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("_", string.Empty)
            .Replace("-", string.Empty);
    }

    private static OrderStatusDto ParseStatus(string status)
    {
        var normalized = NormalizeStatus(status);

        return normalized switch
        {
            "preparado" => OrderStatusDto.Prepared,
            "cancelado" => OrderStatusDto.Cancelled,
            "encamino" => OrderStatusDto.OnTheWay,
            "entregado" => OrderStatusDto.Delivered,
            "noentregado" => OrderStatusDto.NotDelivered,
            _ => throw new ValidationException($"Estado '{status}' no válido")
        };
    }

}
  */
