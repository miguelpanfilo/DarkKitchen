using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/product-imports")]
public class ProductImportsController(IProductImportService productImportService) : ControllerBase
{
    [HttpPost]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult ImportProducts(
        [FromForm] string importerName,
        [FromForm] IFormFile file)
    {
        var request = new ProductImportRequestDto
        {
            ImporterName = importerName, FileName = file.FileName, FileStream = file.OpenReadStream()
        };

        var result = productImportService.ImportProducts(request);

        return Ok(result);
    }
}
