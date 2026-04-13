using System.Security.Claims;
using Contratos;
using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class RoleAuthorizationAttribute(params UserRoles[] allowedRoles) : Attribute, IAuthorizationFilter
{
    private const string ForbiddenMessage = "El usuario no tiene permisos para ejecutar esta acción";
    private readonly UserRoles[] _allowedRoles = allowedRoles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var currentRole = GetRole(context);

        if(string.IsNullOrWhiteSpace(currentRole))
        {
            context.Result = BuildForbiddenResult(ForbiddenMessage);
            return;
        }

        if(!UserHasAllowedRole(currentRole))
        {
            context.Result = BuildForbiddenResult(ForbiddenMessage);
        }
    }

    private static string? GetRole(AuthorizationFilterContext context)
    {
        return context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    private bool UserHasAllowedRole(string currentRole)
    {
        return _allowedRoles.Any(role => role.ToString() == currentRole);
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
