using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Mapeo;

namespace LogicaDeNegocio;

public class ModifyUserService(
    IRepositoryUser repo,
    IPhoneValidator phoneValidator,
    IAdminValidationService adminValidationService)
    : IModifyUserService
{
    private readonly IRepositoryUser _repo = repo;
    private readonly IPhoneValidator _phoneValidator = phoneValidator;
    private readonly IAdminValidationService _adminValidationService = adminValidationService;

    public UsuarioResponseDTO UpdateInternalUser(
        int idUserToModify,
        int idUserLogged,
        ModificarUsuarioRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);

        _adminValidationService.EnsureIsNotSelfAction(
            idUserToModify,
            idUserLogged,
            "Un usuario no puede modificarse a sí mismo");

        var user = _adminValidationService.GetExistingUser(idUserToModify);

        UpdateUserData(user, requestDto);

        return user.MappingUserToResponseDTO();
    }

    public void UpdateUserPassword(
        int idUserToModify,
        int idUserLogged,
        ModificarClaveRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);

        _adminValidationService.EnsureIsNotSelfAction(
            idUserToModify,
            idUserLogged,
            "Un usuario no puede modificarse a sí mismo");

        var usuario = _adminValidationService.GetExistingUser(idUserToModify);

        if(!usuario.VerifyPassword(requestDto.OldPassword))
        {
            throw new ValidationException("La contraseña actual es incorrecta");
        }

        var passwordResult = Password.TryMakePassword(requestDto.NewPassword);

        if(!passwordResult.Success)
        {
            throw new ValidationException(passwordResult.ErrorMessage!);
        }

        usuario.ModifyPassword(passwordResult.Value!);
    }

    private void UpdateUserData(User user, ModificarUsuarioRequestDTO requestDto)
    {
        var nameUpdate = ResolveFinalName(requestDto, user);
        var lastNameUpdate = ResolveFinalLastName(requestDto, user);
        var phoneUpdate = ResolveFinalPhone(requestDto, user);
        var emailUpdate = ResolveFinalEmail(requestDto, user);

        var phoneFinal = BuildFinalPhone(user, phoneUpdate);
        var roleFinal = ResolveFinalRole(requestDto, user);

        ValidateEmailIfChanged(user, emailUpdate);

        var modifyResult = user.ModifyData(
            nameUpdate,
            lastNameUpdate,
            phoneFinal,
            emailUpdate,
            roleFinal);

        if(!modifyResult.Success)
        {
            throw new ValidationException(modifyResult.ErrorMessage!);
        }
    }

    private Phone BuildFinalPhone(User user, string updatedPhone)
    {
        if(updatedPhone == user.Phone.Value)
        {
            return user.Phone;
        }

        return CreateAndValidatePhone(updatedPhone);
    }

    private void ValidateEmailIfChanged(User user, string updatedMail)
    {
        if(!string.Equals(updatedMail, user.Email.ToString(), StringComparison.OrdinalIgnoreCase) &&
            _repo.EmailExist(updatedMail))
        {
            throw new ConflictException("El email ya está registrado");
        }
    }

    // 🔥 VALIDACIONES CORRECTAS (null ≠ vacío)

    private static string ResolveFinalName(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(requestDto.Name == null)
        {
            return user.Name;
        }

        if(string.IsNullOrWhiteSpace(requestDto.Name))
        {
            throw new ValidationException("El nombre no puede estar vacío");
        }

        return requestDto.Name;
    }

    private static string ResolveFinalLastName(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(requestDto.LastName == null)
        {
            return user.LastName;
        }

        if(string.IsNullOrWhiteSpace(requestDto.LastName))
        {
            throw new ValidationException("El apellido no puede estar vacío");
        }

        return requestDto.LastName;
    }

    private static string ResolveFinalEmail(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(requestDto.Email == null)
        {
            return user.Email.ToString();
        }

        if(string.IsNullOrWhiteSpace(requestDto.Email))
        {
            throw new ValidationException("El email no puede estar vacío");
        }

        return requestDto.Email;
    }

    private static string ResolveFinalPhone(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(requestDto.Phone == null)
        {
            return user.Phone.Value;
        }

        if(string.IsNullOrWhiteSpace(requestDto.Phone))
        {
            throw new ValidationException("El teléfono no puede estar vacío");
        }

        return requestDto.Phone;
    }

    private Phone CreateAndValidatePhone(string phone)
    {
        var phoneResult = Phone.TryMakePhone(phone);

        if(!phoneResult.Success)
        {
            throw new ValidationException(phoneResult.ErrorMessage!);
        }

        var newPhone = phoneResult.Value!;

        if(!_phoneValidator.ItsValid(newPhone.Value))
        {
            throw new ValidationException("El teléfono no es válido");
        }

        return newPhone;
    }

    private UserRoles ResolveFinalRole(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(requestDto.Role == null)
        {
            return user.Role;
        }

        if(string.IsNullOrWhiteSpace(requestDto.Role))
        {
            throw new ValidationException("El rol no puede estar vacío");
        }

        return GetValidInternalRole(requestDto.Role);
    }

    private UserRoles GetValidInternalRole(string rol)
    {
        if(!Enum.TryParse<UserRoles>(rol, true, out var rolEnum))
        {
            throw new ValidationException("El rol interno no es válido");
        }

        return rolEnum;
    }
}
