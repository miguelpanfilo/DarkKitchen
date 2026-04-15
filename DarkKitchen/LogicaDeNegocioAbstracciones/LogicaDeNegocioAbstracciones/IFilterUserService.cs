using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IFilterUserService
{
    List<UsuarioResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto);
}
