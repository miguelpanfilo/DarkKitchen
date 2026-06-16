using Domain;

namespace BusinessLogicsAbstractions;

public interface IAdminValidationService
{
    User GetExistingUser(int userId);
    void EnsureUserIsAdmin(int userId);
    void EnsureIsNotSelfAction(int userIdTarget, int userIdLogged, string message);
}
