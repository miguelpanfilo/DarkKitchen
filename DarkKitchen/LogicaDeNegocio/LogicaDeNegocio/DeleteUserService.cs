using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class DeleteUserService : IDeleteUserService
{
    private readonly IRepositoryUser _repositoryUsers;

    public DeleteUserService(IRepositoryUser repositoryUsers)
    {
        _repositoryUsers = repositoryUsers;
    }

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

        if (user.Role != UserRoles.Administrative)
        {
            throw new UsuarioExcepcion("Only an administrative user can perform this action");
        }
    }

    private void ValidationNoAutoDeleting(int idToDelete, int idUserLogged)
    {
        if (idToDelete == idUserLogged)
        {
            throw new UsuarioExcepcion("A user cannot delete themselves");
        }
    }

    private User GetExistingUser(int id)
    {
        var user = _repositoryUsers.FindById(id);

        if (user == null)
        {
            throw new UsuarioExcepcion("User does not exist");
        }

        return user;
    }
}
