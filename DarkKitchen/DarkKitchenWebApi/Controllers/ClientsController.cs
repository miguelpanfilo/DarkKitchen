using BusinessLogicsAbstractions;
using Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/clients")]
public class ClientsController(ISignUpUserService signUpUserService) : ControllerBase
{
    private readonly ISignUpUserService _signUpUserService = signUpUserService;

    [AllowAnonymous]
    [HttpPost]
    public IActionResult CreateClient([FromBody] SignUpClientRequestDTO request)
    {
        var response = _signUpUserService.SignUpClient(request);
        return Created("/api/v1/clients", response);
    }
}
