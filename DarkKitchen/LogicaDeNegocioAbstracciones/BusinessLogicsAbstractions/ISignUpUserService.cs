using Contract;

namespace BusinessLogicsAbstractions;

public interface ISignUpUserService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UserResponseDTO MakeInterUser(int idUserLogged, SignUpInternUserRequestDTO requestDto);
}
