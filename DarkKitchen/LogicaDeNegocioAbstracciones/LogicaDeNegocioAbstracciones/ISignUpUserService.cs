using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface ISignUpUserService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UserResponseDTO MakeInterUser(int idUserLogged, SingUpInternUserRequestDTO requestDto);
}
