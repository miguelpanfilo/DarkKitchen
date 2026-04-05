using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class DeleteUserService : IDeleteUserService
{
    private readonly IRepositoryUser _repositoryUsers;
    private readonly IAdminValidationService _adminValidationService;

    public DeleteUserService(
        IRepositoryUser repositoryUsers,
        IAdminValidationService adminValidationService)
    {
        _repositoryUsers = repositoryUsers;
        _adminValidationService = adminValidationService;
    }

   public void DeleteInterUser(int idToDelete, int idUserLogged)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);
        _adminValidationService.EnsureIsNotSelfAction(
            idToDelete,
            idUserLogged,
            "A user cannot delete themselves"
        );

        var user = _adminValidationService.GetExistingUser(idToDelete);

        _repositoryUsers.Remove(user.Id);
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
