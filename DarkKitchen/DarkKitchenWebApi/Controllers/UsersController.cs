using Contratos;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(ISignUpUserService signUpService) : ControllerBase
{
    private readonly ISignUpUserService _signUpService = signUpService;

    [AllowAnonymous]
    [HttpPost]
    public IActionResult CreateUser([FromBody] SignUpClientRequestDTO request)
    {
        var response = _signUpService.SignUpClient(request);
        return Created("/api/v1/users", response);
    }
}
