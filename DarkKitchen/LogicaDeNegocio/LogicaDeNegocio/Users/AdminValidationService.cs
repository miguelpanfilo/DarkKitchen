using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;

namespace LogicaDeNegocio.Users;

public class AdminValidationService(IRepositoryUser repositoryUser) : IAdminValidationService
{
    private const string UserNotFoundMessage = "El usuario no existe";
    private const string OnlyAdminMessage = "Solo un usuario administrador puede realizar esta acción";
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public User GetExistingUser(int userId)
    {
        var user = _repositoryUser.FindById(userId);

        if(user == null)
        {
            throw new NotFoundException(UserNotFoundMessage);
        }

        return user;
    }

    public void EnsureUserIsAdmin(int userId)
    {
        var user = GetExistingUser(userId);

        if(user.Role != UserRoles.Administrative)
        {
            throw new ForbiddenException(OnlyAdminMessage);
        }
    }

    public void EnsureIsNotSelfAction(int targetUserId, int loggedUserId, string message)
    {
        if(targetUserId == loggedUserId)
        {
            throw new IConflictException(message);
        }
    }
}
