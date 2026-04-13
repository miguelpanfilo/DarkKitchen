using System.Security.Claims;
using Contratos;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            context.Result = BuildUnauthorizedResult("El header de sesión es obligatorio");
            return;
        }

        var sessionIdText = values.FirstOrDefault();
        if(string.IsNullOrWhiteSpace(sessionIdText))
        {
            context.Result = BuildUnauthorizedResult("El header de sesión es obligatorio");
            return;
        }

        if(!Guid.TryParse(sessionIdText, out var sessionId))
        {
            context.Result = BuildUnauthorizedResult("El identificador de sesión es inválido");
            return;
        }

        try
        {
            var current = _sessionValidationService.ValidateSession(sessionId);
            context.HttpContext.Items["CurrentUser"] = current;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, current.Id.ToString()),
                new Claim(ClaimTypes.Role, current.Role)
            };

            var identity = new ClaimsIdentity(claims, "SessionHeader");
            context.HttpContext.User = new ClaimsPrincipal(identity);
        }
        catch(UserAuthenticationException ex)
        {
            context.Result = BuildUnauthorizedResult(ex.Message);
        }
    }

    private static ObjectResult BuildUnauthorizedResult(string message)
    {
        return new ObjectResult(new ResponseDto { ExecutionSuccessful = false, Message = message })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }
}
