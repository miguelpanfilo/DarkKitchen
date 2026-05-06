using BusinessLogicsAbstractions;
using Contract;
using Contract.Users;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.Controllers;

[ApiController]
[Route("api/v1/audits")]
public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    [RoleAuthorization(UserRolesDto.Administrative)]
    public IActionResult GetByFilters([FromQuery] AuditLogFilterRequest request)
    {
        var logs = auditLogService.GetByFilters(request);
        return Ok(logs);
    }
}
