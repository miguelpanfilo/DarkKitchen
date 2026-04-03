using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IConsultaUsuarioService
{
    List<UsuarioResponse> BuscarUsuarios(int idUsuarioLogueado, BuscarUsuariosRequest request);
}
