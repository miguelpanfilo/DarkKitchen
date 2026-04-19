using Contratos.Users;

namespace LogicaDeNegocioAbstracciones.Users;

public interface ILoginService
{
    LoginResponseDTO Login(LoginRequestDTO requestDto);
}
