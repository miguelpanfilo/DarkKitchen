using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IModifyUserService
{
    UserResponseDTO UpdateInternalUser(
        int idUserToModify,
        int isUserLogged,
        ModificarUsuarioRequestDTO requestDto);

    void UpdateUserPassword(
        int idUserToModify,
        int idUserLogged,
        ModificarClaveRequestDTO requestDto);
}
