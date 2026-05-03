using Contract.Users;

namespace BusinessLogicsAbstractions.Users;

public interface ILoginService
{
    LoginResponseDTO Login(LoginRequestDTO requestDto);
}
