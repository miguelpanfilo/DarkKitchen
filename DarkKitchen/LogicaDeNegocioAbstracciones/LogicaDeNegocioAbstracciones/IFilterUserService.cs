using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IFilterUserService
{
    List<UserResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto);
}
