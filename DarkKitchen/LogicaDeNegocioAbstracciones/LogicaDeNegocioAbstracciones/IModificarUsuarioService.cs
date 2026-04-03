using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IModificacionUsuarioService
{
    UsuarioResponse ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequest request);

    void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequest request);
}
