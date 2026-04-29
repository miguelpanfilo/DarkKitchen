using Contratos.Users;

namespace LogicaDeNegocioAbstracciones.Users;

public interface IFilterUserService
{
    List<UserResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto);
}
