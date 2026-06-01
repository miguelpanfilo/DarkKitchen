using Contract;

namespace BusinessLogicsAbstractions;

public interface IUserService
{
    SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto);

    UserResponseDTO MakeUserIntern(int idUserLogged, SignUpInternUserRequestDTO requestDto);

    UserResponseDTO ModifyUserIntern(
        int idUserToModify,
        int idUserLogged,
        ModifyUserRequestDTO requestDto);

    void ModifyPasswordUser(
        int idUserToModify,
        int idUserLogged,
        ModifyPasswordRequestDTO requestDto);

    void DeleteUser(int idUserToDelete, int idUserLogged);

    List<UserResponseDTO> FindUserOnSystem(int idUserLogged, FindUserRequestDTO requestDto);
}
