using Dominio;

namespace LogicaDeNegocioAbstracciones;

public interface IAdminValidationService
{
    User GetExistingUser(int userId);
    void EnsureUserIsAdmin(int userId);
    void EnsureIsNotSelfAction(int userIdTarget, int userIdLogged, string message);
}
