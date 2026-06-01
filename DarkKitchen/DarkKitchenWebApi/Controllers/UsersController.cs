using System.Security.Claims;
using BusinessLogicsAbstractions.Users;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPost]
    public IActionResult CreateInternalUser([FromBody] SignUpInternUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.MakeUserIntern(loggedUserId.Value, request);
        return Created("/api/v1/users", response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPatch("{id}")]
    public IActionResult UpdateInternalUser([FromRoute] int id, [FromBody] ModifyUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.ModifyUserIntern(id, loggedUserId.Value, request);
        return Ok(response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpDelete("{id}")]
    public IActionResult DeleteInternalUser([FromRoute] int id)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        _userService.DeleteUser(id, loggedUserId.Value);
        return NoContent();
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpGet]
    public IActionResult SearchUsers([FromQuery] FindUserRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        var response = _userService.FindUserOnSystem(loggedUserId.Value, request);
        return Ok(response);
    }

    [RoleAuthorization(UserRolesDto.Administrative)]
    [HttpPatch("{id}/password")]
    public IActionResult UpdateInternalUserPassword([FromRoute] int id, [FromBody] ModifyPasswordRequestDTO request)
    {
        var loggedUserId = GetLoggedUserId();
        if(loggedUserId == null)
        {
            return Unauthorized();
        }

        _userService.ModifyPasswordUser(id, loggedUserId.Value, request);
        return NoContent();
    }

    private int? GetLoggedUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(int.TryParse(idClaim, out var id))
        {
            return id;
        }

        return null;
    }
}
