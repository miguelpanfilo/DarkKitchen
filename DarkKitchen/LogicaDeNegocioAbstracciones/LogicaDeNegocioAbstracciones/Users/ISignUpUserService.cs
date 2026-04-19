using Contratos.Users;

namespace LogicaDeNegocioAbstracciones.Users;

public interface ISignUpUserService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UserResponseDTO MakeInterUser(int idUserLogged, SingUpInternUserRequestDTO requestDto);
}
