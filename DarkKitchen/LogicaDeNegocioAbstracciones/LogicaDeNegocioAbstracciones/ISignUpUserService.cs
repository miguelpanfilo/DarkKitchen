using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface ISignUpUserService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UsuarioResponseDTO MakeInterUser(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto);
}
