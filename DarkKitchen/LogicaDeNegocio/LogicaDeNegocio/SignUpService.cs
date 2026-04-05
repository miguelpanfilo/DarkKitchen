using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class SignUpService(IRepositoryUser repositoryUser, IPhoneValidator validatorPhone) : ISignUpUserService
{
    private readonly IRepositoryUser _repositoryUser = repositoryUser;
    private readonly IPhoneValidator _validatorPhone = validatorPhone;

    public SignUpClientResponseDTO SignUpClient(SignUpClientRequestDTO requestDto)
    {
        if(_repositoryUser.EmailExist(requestDto.Email))
        {
            throw new UserException("Email is already registered");
        }

        var phone = MakeAndValidatePhone(requestDto.Phone);

        var usuario = User.MakeClient(
            requestDto.Name,
            requestDto.LastName,
            requestDto.Email,
            phone,
            requestDto.Password);

        var newUser = _repositoryUser.Add(usuario);

        return new SignUpClientResponseDTO
        {
            Name = newUser.Name,
            LastName = newUser.LastName,
            Email = newUser.Email.ToString()
        };
    }

    public UsuarioResponseDTO MakeInterUser(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto)
    {
        ValidateUserAdmin(idUsuarioLogueado);

        var rol = GetInternRoleValid(requestDto.Role);
        var telefono = MakeAndValidatePhone(requestDto.Phone);

        if(_repositoryUser.EmailExist(requestDto.Email))
        {
            throw new UserException("Email is already registered");
        }

        var user = UserMapper.MappingDTOToInternUser(requestDto, telefono, rol);

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

    private void ValidateUserAdmin(int idUsuarioLogueado)
    {
        var user = GetExistingUser(idUsuarioLogueado);

        if(user.Role != UserRoles.Administrative)
        {
            throw new UserException("Only an administrative user can perform this action");
        }
    }

    private User GetExistingUser(int id)
    {
        var user = _repositoryUser.FindById(id);

        if(user == null)
        {
            throw new UserException("User does not exist");
        }

        return user;
    }

    private Phone MakeAndValidatePhone(string telefono)
    {
        var newPhone = Phone.MakePhone(telefono);

        if(!_validatorPhone.ItsValid(newPhone.Value))
        {
            throw new UserException("Phone is not valid");
        }

        return newPhone;
    }
}
