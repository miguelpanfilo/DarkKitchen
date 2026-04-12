using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class AuthenticationFilter : IAuthorizationFilter
{
    private const string SessionHeaderName = "X-Session-Id";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(SessionHeaderName, out var values))
        {
            throw new UserAuthenticationException("El header de sesión es obligatorio");
        }

        var sessionId = values.FirstOrDefault();

        if(string.IsNullOrWhiteSpace(sessionId))
        {
            throw new UserAuthenticationException("El header de sesión es obligatorio");
        }

        if(!Guid.TryParse(sessionId, out _))
        {
            throw new UserAuthenticationException("El identificador de sesión es inválido");
        }
    }
}
