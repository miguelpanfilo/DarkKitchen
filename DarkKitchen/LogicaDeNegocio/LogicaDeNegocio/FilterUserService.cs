using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

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

        var users = _repositoryUsers.Find(requestDto.Name, requestDto.LastName);

        return users
            .Select(u => u.MappingUserToResponseDTO())
            .ToList();
    }
}
