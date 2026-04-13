using Contratos;
using System.Security.Claims;
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
            var user = _sessionValidationService.ValidateSession(sessionId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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
        return new ObjectResult(new ResponseDto
        {
            ExecutionSuccessful = false,
            Message = message
        })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }
}
