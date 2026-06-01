using Contract;

namespace BusinessLogicsAbstractions;

public interface ILoginService
{
    LoginResponseDTO Login(LoginRequestDTO requestDto);
}
