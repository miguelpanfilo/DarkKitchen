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
        int isUserLogged,
        ModificarUsuarioRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(isUserLogged);

        _adminValidationService.EnsureIsNotSelfAction(
            idUserToModify,
            isUserLogged,
            "A user cannot modify itself");

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
            "A user cannot modify itself");

        var usuario = _adminValidationService.GetExistingUser(idUserToModify);

        if(!usuario.VerifyPassword(requestDto.OldPassword))
        {
            throw new ValidationException("Current password is incorrect");
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
            throw new ConflictException("Email is already registered");
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
            throw new ValidationException("Phone is not valid");
        }

        return newPhone;
    }

    private UserRoles ResolveFinalRole(ModificarUsuarioRequestDTO requestDto, User user)
    {
        if(string.IsNullOrWhiteSpace(requestDto.Role))
        {
            return user.Role;
        }

        return GetValidInternalRole(requestDto.Role);
    }

    private UserRoles GetValidInternalRole(string rol)
    {
        if(!Enum.TryParse<UserRoles>(rol, true, out var rolEnum))
        {
            throw new ValidationException("Internal role is not valid");
        }

        return rolEnum;
    }
}
