using BusinessLogicsAbstractions.Imports;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/product-importers")]
public class ProductImportersController(IProductImportService productImportService) : ControllerBase
{
    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetImporters()
    {
        var result = productImportService.GetImporters();
        return Ok(result);
    }
}
