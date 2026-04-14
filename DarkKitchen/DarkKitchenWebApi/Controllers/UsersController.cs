using System.Security.Claims;
using Contratos;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [HttpPost]
    public IActionResult CreateUser([FromBody] SignUpClientRequestDTO request)
    {
        var response = _signUpService.SignUpClient(request);
        return Created("/api/v1/clients", response);
    }

    [HttpPost]
    public IActionResult CreateInternalUser([FromBody] SingUpInternUserRequestDTO request)
    {
        var loggedUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        _signUpService.MakeInterUser(loggedUserId, request);

        return Created(string.Empty, null);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPatch("{id}")]
    public IActionResult UpdateInternalUser([FromRoute] int id, [FromBody] ModificarUsuarioRequestDTO request)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!int.TryParse(idClaim, out var loggedUserId))
        {
            return Unauthorized();
        }

        var response = _modifyUserService.UpdateInternalUser(id, loggedUserId, request);
        return Ok(response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpDelete("{id}")]
    public IActionResult DeleteInternalUser([FromRoute] int id)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!int.TryParse(idClaim, out var loggedUserId))
        {
            return Unauthorized();
        }

        _deleteUserService.DeleteInterUser(id, loggedUserId);
        return NoContent();
    }
}
