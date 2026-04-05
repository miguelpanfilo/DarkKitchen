using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class AdminValidationService : IAdminValidationService
{
    private readonly IRepositoryUser _repositoryUser;

    public AdminValidationService(IRepositoryUser repositoryUser)
    {
        _repositoryUser = repositoryUser;
    }

    public User GetExistingUser(int userId)
    {
        var user = _repositoryUser.FindById(userId);

        if(user == null)
        {
            throw new UserException("User does not exist");
        }

        return user;
    }

    public void EnsureUserIsAdmin(int userId)
    {
        var user = GetExistingUser(userId);

        if(user.Role != UserRoles.Administrative)
        {
            throw new UserException("Only an administrative user can perform this action");
        }
    }

    public void EnsureIsNotSelfAction(int targetUserId, int loggedUserId, string message)
    {
        if(targetUserId == loggedUserId)
        {
            throw new UserException(message);
        }
    }
}
