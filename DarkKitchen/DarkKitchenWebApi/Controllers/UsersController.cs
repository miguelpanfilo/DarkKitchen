using System.Security.Claims;
using Contratos;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(
    ISignUpUserService signUpService,
    IModifyUserService modifyUserService) : ControllerBase
{
    private readonly ISignUpUserService _signUpService = signUpService;
    private readonly IModifyUserService _modifyUserService = modifyUserService;

    [AllowAnonymous]
    [HttpPost]
    public IActionResult CreateUser([FromBody] SignUpClientRequestDTO request)
    {
        var response = _signUpService.SignUpClient(request);
        return Created("/api/v1/users", response);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateInternalUser(int id, [FromBody] ModificarUsuarioRequestDTO request)
    {
        var loggedUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = _modifyUserService.UpdateInternalUser(id, loggedUserId, request);
        return Ok(response);
    }
}
