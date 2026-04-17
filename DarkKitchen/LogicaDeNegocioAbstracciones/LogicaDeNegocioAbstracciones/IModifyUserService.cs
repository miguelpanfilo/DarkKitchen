using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IModifyUserService
{
    UserResponseDTO UpdateInternalUser(
        int idUserToModify,
        int isUserLogged,
        ModifyUserRequestDTO requestDto);

    void UpdateUserPassword(
        int idUserToModify,
        int idUserLogged,
        ModifyPasswordRequestDTO requestDto);
}
