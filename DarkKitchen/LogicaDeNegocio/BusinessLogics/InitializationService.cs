using BusinessLogicsAbstractions;
using DataAccessAbstractions;
using Domain;

namespace BusinessLogics;

public class InitializationService : IInitializationService
{
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminName = "Admin";
    private const string AdminLastName = "System";
    private const string AdminPhone = "099123456";
    private const string AdminPassword = "PasswordSeguraee3!";

    private readonly IShippingTypeRepository _shippingTypeRepository;
    private readonly IRepositoryUser _repositoryUser;

    public InitializationService(IRepositoryUser repositoryUser, IShippingTypeRepository shippingTypeRepository)
    {
        _repositoryUser = repositoryUser;
        _shippingTypeRepository = shippingTypeRepository;
    }

    public void EnsureInitialized()
    {
        EnsureAdminCreated();
        EnsureShippingTypesCreated();
    }

    private void EnsureAdminCreated()
    {
        if(_repositoryUser.EmailExist(AdminEmail))
        {
            return;
        }

        var phoneResult = Phone.TryMakePhone(AdminPhone);
        if(!phoneResult.Success)
        {
            throw new Exception(phoneResult.ErrorMessage);
        }

        var adminResult = User.MakeIntern(
            AdminName,
            AdminLastName,
            AdminEmail,
            phoneResult.Value!,
            AdminPassword,
            UserRoles.Administrative);

        if(!adminResult.Success)
        {
            throw new Exception(adminResult.ErrorMessage);
        }

        _repositoryUser.Add(adminResult.Value!);
    }

    private void EnsureShippingTypesCreated()
    {
        if (_shippingTypeRepository.GetAll().Any())
        {
            return;
        }

        _shippingTypeRepository.Create(ShippingType.MakeShippingType("Express", 100));
        _shippingTypeRepository.Create(ShippingType.MakeShippingType("Envio dentro de 24 horas", 50));
    }
}
