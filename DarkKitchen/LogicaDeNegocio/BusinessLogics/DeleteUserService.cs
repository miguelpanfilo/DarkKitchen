using BusinessLogicsAbstractions.Users;
using DataAccessAbstractions;

namespace BusinessLogics;

public class DeleteUserService(
    IRepositoryUser repositoryUsers,
    IAdminValidationService adminValidationService)
    : IDeleteUserService
{
    private readonly IRepositoryUser _repositoryUsers = repositoryUsers;
    private readonly IAdminValidationService _adminValidationService = adminValidationService;

    public void DeleteInterUser(int idToDelete, int idUserLogged)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);

        _adminValidationService.EnsureIsNotSelfAction(
            idToDelete,
            idUserLogged,
            "Un usuario no puede eliminarse a sí mismo");

        var user = _adminValidationService.GetExistingUser(idToDelete);

        _repositoryUsers.Remove(user.Id);
    }
}
