using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class AuthenticationFilter : IAuthorizationFilter
{
    private const string SessionHeaderName = "X-Session-Id";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(SessionHeaderName, out _))
        {
            throw new UserAuthenticationException("El header de sesión es obligatorio");
        }
    }
}
