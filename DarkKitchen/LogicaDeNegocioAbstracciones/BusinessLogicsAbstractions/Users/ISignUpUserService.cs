using Contract.Users;

namespace BusinessLogicsAbstractions.Users;

public interface ISignUpUserService
{
    SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto);
    UserResponseDTO MakeInterUser(int idUserLogged, SignUpInternUserRequestDTO requestDto);
}
