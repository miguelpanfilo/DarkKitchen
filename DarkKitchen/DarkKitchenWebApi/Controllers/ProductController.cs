using System.Security.Claims;
using BusinessLogicsAbstractions.Products;
using Contract.Products;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Create([FromBody] ProductDTO request)
    {
        var response = productService.Create(request);
        return Created($"/api/v1/products/{response.Id}", response);
    }

    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative, UserRolesDto.Client)]
    public IActionResult Get([FromQuery] ProductSearchRequestDto request)
    {
        var result = productService.GetProducts(request);
        return Ok(result);
    }

    [HttpPatch("{productId}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Update(int productId, ProductPatchDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        var result = productService.Update(productId, request, email);
        return Ok(result);
    }

    [HttpDelete("{productId}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Delete(int productId)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        productService.Delete(productId, email);
        return NoContent();
    }
}
