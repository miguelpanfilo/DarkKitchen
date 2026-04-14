using System.Security.Claims;
using Contratos;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(
    ISignUpUserService signUpService,
    IModifyUserService modifyUserService,
    IDeleteUserService deleteUserService) : ControllerBase
{
    private readonly ISignUpUserService _signUpService = signUpService;
    private readonly IModifyUserService _modifyUserService = modifyUserService;
    private readonly IDeleteUserService _deleteUserService = deleteUserService;

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPost]
    public IActionResult CreateInternalUser([FromBody] SingUpInternUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _signUpService.MakeInterUser(loggedUserId.Value, request);
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

        var response = _modifyUserService.UpdateInternalUser(id, loggedUserId.Value, request);
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

        _deleteUserService.DeleteInterUser(id, loggedUserId.Value);
        return NoContent();
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
