using System.Security.Claims;
using Contract.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class RoleAuthorizationAttribute(params UserRolesDto[] allowedRoles) : Attribute, IAuthorizationFilter
{
    private const string UnauthorizedMessage = "El usuario no está autenticado";
    private const string ForbiddenMessage = "El usuario no tiene permisos para ejecutar esta acción";
    private readonly UserRolesDto[] _allowedRoles = allowedRoles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!IsAuthenticated(context))
        {
            context.Result = BuildUnauthorizedResult(UnauthorizedMessage);
            return;
        }

        var currentRoleText = GetRole(context);

        if(string.IsNullOrWhiteSpace(currentRoleText))
        {
            context.Result = BuildForbiddenResult(ForbiddenMessage);
            return;
        }

        if(!Enum.TryParse<UserRolesDto>(currentRoleText, true, out var currentRole))
        {
            context.Result = BuildForbiddenResult(ForbiddenMessage);
            return;
        }

        if(!UserHasAllowedRole(currentRole))
        {
            context.Result = BuildForbiddenResult(ForbiddenMessage);
        }
    }

    private static bool IsAuthenticated(AuthorizationFilterContext context)
    {
        return context.HttpContext.User.Identity?.IsAuthenticated ?? false;
    }

    private static string? GetRole(AuthorizationFilterContext context)
    {
        return context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    private bool UserHasAllowedRole(UserRolesDto currentRole)
    {
        return _allowedRoles.Contains(currentRole);
    }

    private static ObjectResult BuildUnauthorizedResult(string message)
    {
        return new ObjectResult(new ResponseDto
        {
            ExecutionSuccessful = false,
            Message = message
        })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    private static ObjectResult BuildForbiddenResult(string message)
    {
        return new ObjectResult(new ResponseDto
        {
            ExecutionSuccessful = false,
            Message = message
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
