using Contract.Users;

namespace BusinessLogicsAbstractions.Users;

public interface IFilterUserService
{
    List<UserResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto);
}
