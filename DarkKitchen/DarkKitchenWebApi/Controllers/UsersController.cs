using Contratos;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
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
        try
        {
            var response = _signUpService.SignUpClient(request);
            return Created("/api/v1/users", response);
        }
        catch (SignUpException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
