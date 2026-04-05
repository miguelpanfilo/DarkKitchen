using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class ModifyUserService : IModifyUserService
{
    private readonly IRepositoryUser _repo;
    private readonly IPhoneValidator _phoneValidator;

    public ModifyUserService(IRepositoryUser repo, IPhoneValidator phoneValidator)
    {
        _repo = repo;
        _phoneValidator = phoneValidator;
    }

    public UsuarioResponseDTO UpdateInternalUser(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto)
    {
        ValidateUserIsAdmin(idUsuarioLogueado);
        ValidateNotSelfModification(idUsuarioAModificar, idUsuarioLogueado);

        var usuario = GetExistingUser(idUsuarioAModificar);

        UpdateUserData(usuario, requestDto);

        return usuario.MappingUserToResponseDTO();
    }

    public void UpdateUserPassword(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto)
    {
        ValidateUserIsAdmin(idUsuarioLogueado);

        var usuario = GetExistingUser(idUsuarioAModificar);

        if (!usuario.VerifyPassword(requestDto.OldPassword))
        {
            throw new UserException("Current password is incorrect");
        }

        usuario.ModifyPassword(Password.MakePassword(requestDto.NewPassword));
    }

    private void UpdateUserData(User user, ModificarUsuarioRequestDTO requestDto)
    {
        var nombreActualizado = ResolveFinalName(requestDto, user);
        var apellidoActualizado = ResolveFinalLastName(requestDto, user);
        var telefonoActualizado = ResolveFinalPhone(requestDto, user);
        var emailActualizado = ResolveFinalEmail(requestDto, user);

        var telefonoFinal = BuildFinalPhone(user, telefonoActualizado);
        var rolFinal = ResolveFinalRole(requestDto, user);
        ValidateEmailIfChanged(user, emailActualizado);

        user.ModifyData(nombreActualizado, apellidoActualizado, telefonoFinal, emailActualizado, rolFinal);
    }

    private Phone BuildFinalPhone(User user, string telefonoActualizado)
    {
        if (telefonoActualizado == user.Phone.Value)
        {
            return user.Phone;
        }

        return CreateAndValidatePhone(telefonoActualizado);
    }

    private void ValidateEmailIfChanged(User user, string emailActualizado)
    {
        if(!string.Equals(emailActualizado, user.Email.ToString(), StringComparison.OrdinalIgnoreCase)
           && _repo.EmailExist(emailActualizado))
        {
            throw new UserException("Email is already registered");
        }
    }

    private static string ResolveFinalEmail(ModificarUsuarioRequestDTO requestDto, User user)
    {
        return string.IsNullOrWhiteSpace(requestDto.Email)
            ? user.Email.ToString()
            : requestDto.Email;
    }

    private static string ResolveFinalLastName(ModificarUsuarioRequestDTO requestDto, User user)
    {
        return string.IsNullOrWhiteSpace(requestDto.LastName)
            ? user.LastName
            : requestDto.LastName;
    }

    private static string ResolveFinalName(ModificarUsuarioRequestDTO requestDto, User user)
    {
        return string.IsNullOrWhiteSpace(requestDto.Name)
            ? user.Name
            : requestDto.Name;
    }

    private static string ResolveFinalPhone(ModificarUsuarioRequestDTO requestDto, User user)
    {
        return string.IsNullOrWhiteSpace(requestDto.Phone)
            ? user.Phone.Value
            : requestDto.Phone;
    }

    private void ValidateUserIsAdmin(int idUsuarioLogueado)
    {
        var usuario = GetExistingUser(idUsuarioLogueado);

        if (usuario.Role != UserRoles.Administrative)
        {
            throw new UserException("Only an administrative user can perform this action");
        }
    }

    private void ValidateNotSelfModification(int idUsuarioAModificar, int idUsuarioLogueado)
    {
        if (idUsuarioAModificar == idUsuarioLogueado)
        {
            throw new UserException("A user cannot modify itself");
        }
    }

    private User GetExistingUser(int id)
    {
        var usuario = _repo.FindById(id);

        if(usuario == null)
        {
            throw new UserException("User does not exist");
        }

        return usuario;
    }

    private Phone CreateAndValidatePhone(string telefono)
    {
        var telefonoCreado = Phone.MakePhone(telefono);

        if (!_phoneValidator.ItsValid(telefonoCreado.Value))
        {
            throw new UserException("Phone is not valid");
        }

        return telefonoCreado;
    }

    private UserRoles ResolveFinalRole(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if (string.IsNullOrWhiteSpace(requestDto.Role))
        {
            return user.Role;
        }

        return GetValidInternalRole(requestDto.Role);
    }

    private UserRoles GetValidInternalRole(string rol)
    {
        if(!Enum.TryParse<UserRoles>(rol, true, out var rolEnum))
        {
            throw new UserException("Internal role is not valid");
        }

        return rolEnum;
    }
}
