using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
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
            throw new UserException("Email is already registered");
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
            throw new UserException("Email is already registered");
        }

        var user = UserMapper.MappingDTOToInternUser(requestDto, phone, role);

        var newUser = _repositoryUser.Add(user);

        return newUser.MappingUserToResponseDTO();
    }

    private UserRoles GetInternRoleValid(string rol)
    {
        if(!Enum.TryParse<UserRoles>(rol, true, out var rolEnum))
        {
            throw new UserException("Internal role is not valid");
        }

        return rolEnum;
    }

    private Phone MakeAndValidatePhone(string phone)
    {
        var newPhone = Phone.MakePhone(phone);

        if(!_validatorPhone.ItsValid(newPhone.Value))
        {
            throw new UserException("Phone is not valid");
        }

        return newPhone;
    }
}
