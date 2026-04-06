using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Mapeo;

namespace LogicaDeNegocio;

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
            throw new SignUpException("Email is already registered");
        }

        var phone = MakeAndValidatePhone(requestDto.Phone);

        var user = User.MakeClient(
            requestDto.Name,
            requestDto.LastName,
            requestDto.Email,
            phone,
            requestDto.Password);

        var newUser = _repositoryUser.Add(user);

        return new SignUpClientResponseDTO
        {
            Name = newUser.Name,
            LastName = newUser.LastName,
            Email = newUser.Email.ToString(),
        };
    }

    public UsuarioResponseDTO MakeInterUser(int idUserLogged, SingUpInternUserRequestDTO requestDto)
    {
        _adminValidationService.EnsureUserIsAdmin(idUserLogged);

        var role = GetInternRoleValid(requestDto.Role);
        var phone = MakeAndValidatePhone(requestDto.Phone);

        if(_repositoryUser.EmailExist(requestDto.Email))
        {
            throw new SignUpException("Email is already registered");
        }

        var user = UserMapper.MappingDTOToInternUser(requestDto, phone, role);

        var newUser = _repositoryUser.Add(user);

        return newUser.MappingUserToResponseDTO();
    }

    private UserRoles GetInternRoleValid(string role)
    {
        if(!Enum.TryParse<UserRoles>(role, true, out var rolEnum))
        {
            throw new SignUpException("Internal role is not valid");
        }

        return rolEnum;
    }

    private Phone MakeAndValidatePhone(string phone)
    {
        var newPhone = Phone.MakePhone(phone);

        if(!_validatorPhone.ItsValid(newPhone.Value))
        {
            throw new SignUpValidationException("Phone is not valid");
        }

        return newPhone;
    }
}
