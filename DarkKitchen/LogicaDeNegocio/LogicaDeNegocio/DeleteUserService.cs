using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class DeleteUserService(IRepositoryUser repositoryUsers) : IDeleteUserService
{
    private readonly IRepositoryUser _repositoryUsers = repositoryUsers;

    public void DeleteInterUser(int idToDelete, int idUserLogged)
    {
        ValidationUSerAdmin(idUserLogged);
        ValidationNoAutoDeleting(idToDelete, idUserLogged);

        var user = GetExistingUser(idToDelete);

        _repositoryUsers.Remove(user.Id);
    }

    private void ValidationUSerAdmin(int idUserLogged)
    {
        var user = GetExistingUser(idUserLogged);

        if(user.Role != UserRoles.Administrative)
        {
            throw new UserException("Only an administrative user can perform this action");
        }
    }

    private void ValidationNoAutoDeleting(int idToDelete, int idUserLogged)
    {
        if(idToDelete == idUserLogged)
        {
            throw new UserException("A user cannot delete themselves");
        }
    }

    private User GetExistingUser(int id)
    {
        var user = _repositoryUsers.FindById(id);

        if(user == null)
        {
            throw new UserException("User does not exist");
        }

        return user;
    }
}
