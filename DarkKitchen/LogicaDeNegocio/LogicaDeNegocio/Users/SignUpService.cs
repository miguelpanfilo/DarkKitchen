using AlmacenamientoAbstracciones;
using Contratos.Users;
using Dominio;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;
using Mapeo;

namespace LogicaDeNegocio.Users;

public class SignUpService(
    IRepositoryUser repositoryUser,
    IPhoneValidator validatorPhone,
    IAdminValidationService adminValidationService)
    : ISignUpUserService
{
    private readonly IRepositoryUser _repositoryUser = repositoryUser;
    private readonly IPhoneValidator _validatorPhone = validatorPhone;
    private readonly IAdminValidationService _adminValidationService = adminValidationService;

    public SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto)
    {
        if(_repositoryUser.EmailExist(requestDto.Email))
        {
            throw new IConflictException("Email ya registrado");
        }

        var phone = MakeAndValidatePhone(requestDto.Phone);

        var userResult = User.MakeClient(
            requestDto.Name,
            requestDto.LastName,
            requestDto.Email,
            phone,
            requestDto.Password);

        if(!userResult.Success)
        {
            throw new ValidationException(userResult.ErrorMessage!);
        }

        var newUser = _repositoryUser.Add(userResult.Value!);

        return new SignUpClientResponseDTO
        {
            Name = newUser.Name,
            LastName = newUser.LastName,
            Email = newUser.Email.ToString()
        };
    }

    public UserResponseDTO MakeInterUser(int idUserLogged, SingUpInternUserRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);

        var role = GetInternRoleValid(requestDto.Role);
        var phone = MakeAndValidatePhone(requestDto.Phone);

        var userResult = requestDto.MappingDTOToInternUser(phone, role);

        if(!userResult.Success)
        {
            throw new ValidationException(userResult.ErrorMessage!);
        }

        if(_repositoryUser.EmailExist(requestDto.Email))
        {
            throw new IConflictException("Email ya registrado");
        }

        var newUser = _repositoryUser.Add(userResult.Value!);

        return newUser.MappingUserToResponseDTO();
    }

    private UserRoles GetInternRoleValid(string role)
    {
        if(!Enum.TryParse<UserRoles>(role, true, out var rolEnum))
        {
            throw new ValidationException("El rol no es válido");
        }

        return rolEnum;
    }

    private Phone MakeAndValidatePhone(string phone)
    {
        var phoneResult = Phone.TryMakePhone(phone);

        if(!phoneResult.Success)
        {
            throw new ValidationException(phoneResult.ErrorMessage!);
        }

        var newPhone = phoneResult.Value!;

        if(!_validatorPhone.ItsValid(newPhone.Value))
        {
            throw new ValidationException("Telefono no valido");
        }

        return newPhone;
    }
}
