using BusinessLogicsAbstractions.Imports;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/product-importers")]
public class ProductImportersController(IProductImporterQueryService productImporterQueryService) : ControllerBase
{
    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetImporters()
    {
        var result = productImporterQueryService.GetImporters();
        return Ok(result);
    }
}
