using Contratos;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly ISignUpUserService _signUpService;

    public UsersController(ISignUpUserService signUpService)
    {
        _signUpService = signUpService;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] SignUpClientRequestDTO request)
    {
        var response = _signUpService.SignUpClient(request);
        return Created("/api/v1/users", response);
    }
}
