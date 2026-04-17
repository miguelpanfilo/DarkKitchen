using Contratos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public sealed class RoleFilter : Attribute, IAuthorizationFilter
{
    private readonly UserRolesDto[] _allowedRoles;

    public RoleFilter(params UserRolesDto[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.Items["CurrentUser"] as CurrentUserDto;

        if(user is null || !_allowedRoles.Contains(user.Role))
        {
            context.Result = new ObjectResult(new ResponseDto
            {
                ExecutionSuccessful = false,
                Message = "No tiene permisos para realizar esta acción"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
