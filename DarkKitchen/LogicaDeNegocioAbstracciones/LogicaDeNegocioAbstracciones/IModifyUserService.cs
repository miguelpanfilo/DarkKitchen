using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IModifyUserService
{
    UsuarioResponseDTO UpdateInternalUser(
        int idUserToModify,
        int isUserLogged,
        ModificarUsuarioRequestDTO requestDto);

    void UpdateUserPassword(
        int idUserToModify,
        int idUserLogged,
        ModificarClaveRequestDTO requestDto);
}
