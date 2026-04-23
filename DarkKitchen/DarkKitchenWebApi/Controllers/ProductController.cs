using Contratos.Products;
using Contratos.Users;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Products;
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
        return Created(string.Empty, response);
    }

    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative, UserRolesDto.Client)]
    public IActionResult Get([FromQuery] ProductSearchRequestDto request)
    {
        var result = productService.GetProducts(request);
        return Ok(result);
    }
}
