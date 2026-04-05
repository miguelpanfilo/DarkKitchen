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
        return _repositoryUser.FindById(userId)!;
    }
}
