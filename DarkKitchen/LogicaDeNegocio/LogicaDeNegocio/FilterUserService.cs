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

    public List<UsuarioResponseDTO> BuscarUsuarios(int idUserLogin, FindUserRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogin);

        var users = _repositoryUsers.GetAll();
        var filteredUsers = new List<UsuarioResponseDTO>();

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

    private static bool MatchName(User user, string? name)
    {
        return string.IsNullOrWhiteSpace(name) ||
               string.Equals(user.Name, name.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchLastName(User user, string? lastname)
    {
        return string.IsNullOrWhiteSpace(lastname) ||
               string.Equals(user.LastName, lastname.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
