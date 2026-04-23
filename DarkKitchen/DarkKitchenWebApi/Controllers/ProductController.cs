using Contratos.Products;
using LogicaDeNegocioAbstracciones.Products;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public CreatedResult Create([FromBody] ProductDTO request)
    {
        var response = productService.Create(request);
        return Created(string.Empty, response);
    }
}
