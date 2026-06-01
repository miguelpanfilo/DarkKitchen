using Contract;

namespace BusinessLogicsAbstractions;

public interface IFilterUserService
{
    List<UserResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto);
}
