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
}
