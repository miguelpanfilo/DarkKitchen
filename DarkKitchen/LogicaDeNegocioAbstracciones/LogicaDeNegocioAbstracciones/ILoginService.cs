using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface ILoginService
{
    LoginResponseDTO Login(LoginRequestDTO requestDto);
}
