using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IAltaUsuarioService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UsuarioResponseDTO MakeInterUser(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto);
}
