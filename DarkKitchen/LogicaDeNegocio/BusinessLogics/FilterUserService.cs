using BusinessLogicsAbstractions;
using Contract;
using DataAccessAbstractions;
using Domain;
using Mappers;

namespace BusinessLogics;

public class FilterUserService(
    IRepositoryUser repositoryUsers,
    IAdminValidationService adminValidationService)
    : IFilterUserService
{
    private readonly IRepositoryUser _repositoryUsers = repositoryUsers;
    private readonly IAdminValidationService _adminValidationService = adminValidationService;

    public List<UserResponseDTO> FindUser(int idUserLogin, FindUserRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogin);

        var users = _repositoryUsers.GetAll();
        var filteredUsers = new List<UserResponseDTO>();

        foreach(var u in users)
        {
            if(MatchName(u, requestDto.Name) &&
               MatchLastName(u, requestDto.LastName))
            {
                filteredUsers.Add(u.MappingUserToResponseDTO());
            }
        }

        return filteredUsers;
    }

    public UserResponseDTO GetByIdIncludingInactive(int id)
    {
        var user = _repositoryUsers.FindByIdIncludingInactive(id);

        if (user == null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {id}");
        }

        return user.MappingUserToResponseDTO();
    }

    private static bool MatchName(User user, string? name)
    {
        return string.IsNullOrWhiteSpace(name) ||
               user.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchLastName(User user, string? lastname)
    {
        return string.IsNullOrWhiteSpace(lastname) ||
               user.LastName.Contains(lastname.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
