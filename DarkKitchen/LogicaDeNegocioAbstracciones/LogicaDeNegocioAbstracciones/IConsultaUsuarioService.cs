using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IConsultaUsuarioService
{
    List<UsuarioResponseDTO> BuscarUsuarios(int idUserLogin, FindUserRequestDTO requestDto);
}
