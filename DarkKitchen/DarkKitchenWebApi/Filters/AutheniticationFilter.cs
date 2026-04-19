using System.Security.Claims;
using Contratos;
using Contratos.Users;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class AuthenticationFilter(ISessionsValidationService sessionsValidationService)
    : IAuthorizationFilter
{
    private const string SessionHeaderName = "X-Session-Id";
    private const string CurrentUserItemKey = "CurrentUser";
    private readonly ISessionsValidationService _sessionsValidationService = sessionsValidationService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if(hasAllowAnonymous)
        {
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(SessionHeaderName, out var values) || !values.Any() || string.IsNullOrWhiteSpace(values.FirstOrDefault()))
        {
            context.Result = BuildUnauthorizedResult("El header de sesión es obligatorio");
            return;
        }

        var sessionIdText = values.FirstOrDefault();

        if(!Guid.TryParse(sessionIdText, out var sessionId))
        {
            context.Result = BuildUnauthorizedResult("El identificador de sesión es inválido");
            return;
        }

        try
        {
            var current = _sessionsValidationService.ValidateSession(sessionId);
            context.HttpContext.Items[CurrentUserItemKey] = current;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, current.Id.ToString()),
                new Claim(ClaimTypes.Role, current.Role.ToString()),
                new Claim(ClaimTypes.Email, current.Email)
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
