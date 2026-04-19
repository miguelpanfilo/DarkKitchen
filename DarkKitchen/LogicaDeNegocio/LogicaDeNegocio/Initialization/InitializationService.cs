using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones.Users;

namespace LogicaDeNegocio.Initialization;

public class InitializationService : IInitializationService
{
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminName = "Admin";
    private const string AdminLastName = "System";
    private const string AdminPhone = "099123456";
    private const string AdminPassword = "PasswordSeguraee3!";

    private readonly IRepositoryUser _repositoryUser;

    public InitializationService(IRepositoryUser repositoryUser)
    {
        _repositoryUser = repositoryUser;
    }

    public void EnsureInitialized()
    {
        var admin = _repositoryUser.FindByEmail(AdminEmail);
        if(admin == null)
        {
            CreateAdminUser();
        }
    }

    private void CreateAdminUser()
    {
        var phoneResult = Phone.TryMakePhone(AdminPhone);
        if(!phoneResult.Success)
        {
            throw new InvalidOperationException("No se pudo crear el teléfono del admin inicial.");
        }

        var userResult = User.MakeIntern(
            AdminName,
            AdminLastName,
            AdminEmail,
            phoneResult.Value!,
            AdminPassword,
            UserRoles.Administrative);

        if(!userResult.Success)
        {
            throw new InvalidOperationException("No se pudo crear el usuario admin inicial.");
        }

        _repositoryUser.Add(userResult.Value!);
    }
}
