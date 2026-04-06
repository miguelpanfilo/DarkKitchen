using Contratos;
using LogicaDeNegocioAbstracciones;
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
    public IActionResult Login(LoginRequestDTO request)
    {
        var response = _loginService.Login(request);
        return Ok(response);
    }
}
