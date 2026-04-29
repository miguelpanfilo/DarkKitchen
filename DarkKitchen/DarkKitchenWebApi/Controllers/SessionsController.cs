using Contratos.Users;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/sessions")]
public class SessionsController(ILoginService loginService) : ControllerBase
{
    private readonly ILoginService _loginService = loginService;

    [AllowAnonymous]
    [HttpPost]
    public IActionResult CreateSession([FromBody] LoginRequestDTO request)
    {
        var response = _loginService.Login(request);
        return Ok(response);
    }
}
