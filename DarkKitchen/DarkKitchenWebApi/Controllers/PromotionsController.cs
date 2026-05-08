using System.Security.Claims;
using BusinessLogicsAbstractions.Promotions;
using Contract.Promotions;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/promotions")]
public class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [HttpPost]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Create([FromBody] PromotionRequestDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        var response = _promotionService.Create(request, email);
        return Created($"/api/v1/promotions/{response.Id}", response);
    }

    [HttpPatch("{id}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Update([FromRoute] int id, [FromBody] PromotionPatchDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        var response = _promotionService.Update(id, request, email);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Delete([FromRoute] int id)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        _promotionService.Delete(id, email);
        return NoContent();
    }

    [HttpPost("{promotionId}/products/{productId}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult AssociateProduct([FromRoute] int promotionId, [FromRoute] int productId)
    {
        var response = _promotionService.AssociateProduct(promotionId, productId);
        return Ok(response);
    }

    [HttpDelete("{promotionId}/products/{productId}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult DisassociateProduct([FromRoute] int promotionId, [FromRoute] int productId)
    {
        var response = _promotionService.DisassociateProduct(promotionId, productId);
        return Ok(response);
    }

    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative, UserRolesDto.Client)]
    public IActionResult Get([FromQuery] PromotionSearchRequestDto request)
    {
        var result = _promotionService.GetPromotions(request);
        return Ok(result);
    }
}
