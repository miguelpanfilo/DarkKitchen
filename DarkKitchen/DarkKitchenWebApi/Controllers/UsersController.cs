using System.Security.Claims;
using Contratos;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPost]
    public IActionResult CreateInternalUser([FromBody] SingUpInternUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.CrearUsuarioInterno(loggedUserId.Value, request);
        return Created("/api/v1/users", response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPatch("{id}")]
    public IActionResult UpdateInternalUser([FromRoute] int id, [FromBody] ModificarUsuarioRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.ModificarUsuarioInterno(id, loggedUserId.Value, request);
        return Ok(response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpDelete("{id}")]
    public IActionResult DeleteInternalUser([FromRoute] int id)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        _userService.EliminarUsuarioInterno(id, loggedUserId.Value);
        return NoContent();
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpGet]
    public IActionResult SearchUsers([FromQuery] FindUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if (loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.BuscarUsuarios(loggedUserId.Value, request);
        return Ok(response);
    }

    private int? GetLoggedUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(int.TryParse(idClaim, out var id))
        {
            return id;
        }

        return null;
    }
}
