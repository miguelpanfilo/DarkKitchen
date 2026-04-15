using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class InitializationService : IInitializationService
{
    private readonly IRepositoryUser _repositoryUser;

    public InitializationService(IRepositoryUser repositoryUser)
    {
        _repositoryUser = repositoryUser;
    }

    public void EnsureInitialized()
    {
        var admin = _repositoryUser.FindByEmail("admin@gmail.com");
        if(admin == null)
        {
            var phoneResult = Phone.TryMakePhone("099123456");
            if(!phoneResult.Success)
            {
                return;
            }

            var userResult = User.MakeIntern(
                "Admin",
                "System",
                "admin@gmail.com",
                phoneResult.Value!,
                "PasswordSeguraee3!",
                UserRoles.Administrative);

            if(!userResult.Success)
            {
                return;
            }

            _repositoryUser.Add(userResult.Value!);
        }
    }
}
