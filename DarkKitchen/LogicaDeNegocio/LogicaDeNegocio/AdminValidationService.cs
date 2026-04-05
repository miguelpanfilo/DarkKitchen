using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class AdminValidationService(IRepositoryUser repositoryUser) : IAdminValidationService
{
    private const string UserNotFoundMessage = "User does not exist";
    private const string OnlyAdminMessage = "Only an administrative user can perform this action";
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public User GetExistingUser(int userId)
    {
        var user = _repositoryUser.FindById(userId);

        if(user == null)
        {
            throw new UserException(UserNotFoundMessage);
        }

        return user;
    }

    public void EnsureUserIsAdmin(int userId)
    {
        var user = GetExistingUser(userId);

        if(user.Role != UserRoles.Administrative)
        {
            throw new UserException(OnlyAdminMessage);
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
