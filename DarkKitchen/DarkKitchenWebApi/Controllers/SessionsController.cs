using Contratos;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/sessions")]
public class SessionsController : ControllerBase
{
    private readonly ILoginService _loginService;

    public SessionsController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public IActionResult CreateSession([FromBody] LoginRequestDTO request)
    {
        var response = _loginService.Login(request);
        return Ok(response);
    }
}
