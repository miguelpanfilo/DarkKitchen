
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class FilterUserService : IFilterUserService
{
    private readonly IRepositoryUser _repositoryUsers;

    public FilterUserService(IRepositoryUser repositoryUsers)
    {
        _repositoryUsers = repositoryUsers;
    }

    public List<UsuarioResponseDTO> BuscarUsuarios(int idUserLogin, FindUserRequestDTO requestDto)
    {
        var userLogin = GetExistingUser(idUserLogin);

        if (userLogin.Role != UserRoles.Administrative)
        {
            throw new UserException("Only an administrative user can perform this action");
        }

        var users = _repositoryUsers.GetAll();
        var filterUsers = new List<UsuarioResponseDTO>();

        foreach (var u in users)
        {
            if (MatchName(u, requestDto.Name) && MatchLastName(u, requestDto.LastName))
            {
                filterUsers.Add(u.MappingUserToResponseDTO());
            }
        }

        return filterUsers;
    }

    private User GetExistingUser(int id)
    {
        var user = _repositoryUsers.FindById(id);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }

        return user;
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
