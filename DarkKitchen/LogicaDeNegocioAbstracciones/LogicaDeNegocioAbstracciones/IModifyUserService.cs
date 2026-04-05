using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IModifyUserService
{
    UsuarioResponseDTO UpdateInternalUser(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto);

    void UpdateUserPassword(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto);
}
