using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

public class ProductImportsController(IProductImportService productImportService) : ControllerBase
{
    [HttpPost]
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
