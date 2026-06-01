using Contract.Users;

namespace BusinessLogicsAbstractions;

public interface ILoginService
{
    LoginResponseDTO Login(LoginRequestDTO requestDto);
}
