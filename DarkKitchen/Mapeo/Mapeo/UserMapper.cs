using Contratos;
using Dominio;
using Dominio.Excepciones;

namespace Mapeo;

public static class UserMapper
{
    public static User MappingDTOToInternUser(
        this SingUpInternUserRequestDTO requestDto,
        Phone phone,
        UserRoles rol)
    {
        return User.MakeIntern(
            requestDto.Name,
            requestDto.LastName,
            requestDto.Email,
            phone,
            requestDto.Password,
            rol);
    }

    public static UsuarioResponseDTO MappingUserToResponseDTO(this User user)
    {
        if(user == null)
        {
            throw new UserException("User cannot be null");
        }

        return new UsuarioResponseDTO
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email.ToString(),
            Role = user.Role.ToString(),
            Phone = user.Phone.Value
        };
    }
}
