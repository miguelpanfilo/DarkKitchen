using BusinessLogicsAbstractions.Users;
using DataAccessAbstractions;
using Domain;

namespace BusinessLogics.Initialization;

public class InitializationService : IInitializationService
{
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminName = "Admin";
    private const string AdminLastName = "System";
    private const string AdminPhone = "099123456";
    private const string AdminPassword = "PasswordSeguraee3!";

    private readonly IRepositoryUser _repositoryUser;

    public InitializationService(
        IRepositoryUser repositoryUser)
    {
        _repositoryUser = repositoryUser;
    }

    public void EnsureInitialized()
    {
        EnsureAdminCreated();
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
}
