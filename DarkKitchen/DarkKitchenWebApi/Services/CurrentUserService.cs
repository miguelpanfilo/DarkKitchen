using System.Security.Claims;
using BusinessLogicsAbstractions;

namespace DarkKitchen.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetEmail()
    {
        return _httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.Email)!;
    }
}
