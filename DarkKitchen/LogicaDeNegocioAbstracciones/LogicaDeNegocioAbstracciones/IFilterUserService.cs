using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IFilterUserService
{
    List<UsuarioResponseDTO> BuscarUsuarios(int idUserLogin, FindUserRequestDTO requestDto);
}
