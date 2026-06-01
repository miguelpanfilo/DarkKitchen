using BusinessLogicsAbstractions;
using Contract.Users;

namespace BusinessLogics;

public class UserService(
    ISignUpUserService signUpUserService,
    IModifyUserService modifyUserService,
    IDeleteUserService deleteUserService,
    IFilterUserService filterUserService) : IUserService
{
    private readonly ISignUpUserService _signUpUserService = signUpUserService;
    private readonly IModifyUserService _modifyUserService = modifyUserService;
    private readonly IDeleteUserService _deleteUserService = deleteUserService;
    private readonly IFilterUserService _filterUserService = filterUserService;

    public SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto)
    {
        return _signUpUserService.SignUpClient(requestDto);
    }

    public UserResponseDTO MakeUserIntern(int idUserLogged, SignUpInternUserRequestDTO requestDto)
    {
        return _signUpUserService.MakeInterUser(idUserLogged, requestDto);
    }

    public UserResponseDTO ModifyUserIntern(
        int idUserToModify,
        int idUserLogged,
        ModifyUserRequestDTO requestDto)
    {
        return _modifyUserService.UpdateInternalUser(
            idUserToModify,
            idUserLogged,
            requestDto);
    }

    public void ModifyPasswordUser(
        int idUserToModify,
        int idUserLogged,
        ModifyPasswordRequestDTO requestDto)
    {
        _modifyUserService.UpdateUserPassword(
            idUserToModify,
            idUserLogged,
            requestDto);
    }

    public void DeleteUser(int idUserToDelete, int idUserLogged)
    {
        _deleteUserService.DeleteInterUser(idUserToDelete, idUserLogged);
    }

    public List<UserResponseDTO> FindUserOnSystem(int idUserLogged, FindUserRequestDTO requestDto)
    {
        return _filterUserService.FindUser(idUserLogged, requestDto);
    }
}
