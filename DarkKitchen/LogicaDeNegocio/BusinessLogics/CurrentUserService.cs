using System.Security.Claims;
using BusinessLogicsAbstractions;
using Microsoft.AspNetCore.Http;

namespace BusinessLogics;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) :
    ICurrentUserService
{
    public string GetEmail()
    {
        var email = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.Email)?
            .Value;

        return string.IsNullOrWhiteSpace(email)
            ? "No hay usuario logueado"
            : email;
    }
}
