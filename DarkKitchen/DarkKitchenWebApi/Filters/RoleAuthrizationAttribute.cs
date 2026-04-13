using System.Security.Claims;
using Contratos;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class RoleAuthorizationAttribute(params UserRoles[] allowedRoles) : Attribute, IAuthorizationFilter
{
    private readonly UserRoles[] _allowedRoles = allowedRoles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);

        if(roleClaim == null || string.IsNullOrWhiteSpace(roleClaim.Value))
        {
            context.Result = BuildForbiddenResult("El usuario no tiene permisos para ejecutar esta acción");
            return;
        }

        var currentRoleText = roleClaim.Value;

        var hasPermission = _allowedRoles.Any(role => role.ToString() == currentRoleText);

        if(!hasPermission)
        {
            context.Result = BuildForbiddenResult("El usuario no tiene permisos para ejecutar esta acción");
        }
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
