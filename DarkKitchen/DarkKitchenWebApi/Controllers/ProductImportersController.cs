using BusinessLogicsAbstractions.Imports;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/product-importers")]
public class ProductImportController(IProductImportService productImportService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetImporters()
    {
        var result = productImportService.GetImporters();
        return Ok(result);
    }
}
