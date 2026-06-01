using System.Security.Claims;
using BusinessLogicsAbstractions;
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
        var result = productService.Update(productId, request);
        return Ok(result);
    }

    [HttpDelete("{productId}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Delete(int productId)
    {
        productService.Delete(productId);
        return NoContent();
    }
}
