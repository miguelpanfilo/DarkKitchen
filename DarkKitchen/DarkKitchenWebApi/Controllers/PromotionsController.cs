using Contratos.Promotions;
using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Promotions;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/promotions")]
public class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    private CurrentUserDto CurrentUser =>
        (CurrentUserDto)HttpContext.Items["CurrentUser"]!;

    [HttpPost]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Create([FromBody] PromotionRequestDto request)
    {
        var response = _promotionService.Create(request);
        return Created(string.Empty, response);
    }
}
