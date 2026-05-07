using BusinessLogicsAbstractions.ShippingTypes;
using Contract.ShippingTypes;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/shipping-types")]
public class ShippingTypesController(IShippingTypeService shippingTypeService) : ControllerBase
{
    private readonly IShippingTypeService _shippingTypeService = shippingTypeService;

    [HttpPost]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Create([FromBody] ShippingTypeRequestDto request)
    {
        var response = _shippingTypeService.CreateShippingType(request);
        return Created($"/api/v1/shipping-types/{response.Id}", response);
    }

    [HttpPatch("{id}")]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult Update([FromRoute] int id, [FromBody] ShippingTypePatchDto request)
    {
        var response = _shippingTypeService.UpdateShippingType(id, request);
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = _shippingTypeService.GetAllShippingTypes();
        return Ok(response);
    }
}
