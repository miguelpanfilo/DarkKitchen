using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class AuthenticationFilter(ISessionValidationService sessionValidationService)
    : IAuthorizationFilter
{
    private const string SessionHeaderName = "X-Session-Id";
    private readonly ISessionValidationService _sessionValidationService = sessionValidationService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if(hasAllowAnonymous)
        {
            return;
        }

        if(!context.HttpContext.Request.Headers.TryGetValue(SessionHeaderName, out var values))
        {
            throw new UserAuthenticationException("El header de sesión es obligatorio");
        }

        var sessionID = values.FirstOrDefault();

        if(string.IsNullOrWhiteSpace(sessionID))
        {
            throw new UserAuthenticationException("El header de sesión es obligatorio");
        }

        if(!Guid.TryParse(sessionID, out var sessionId))
        {
            throw new UserAuthenticationException("El identificador de sesión es inválido");
        }

        _sessionValidationService.ValidateSession(sessionId);
    }
}
