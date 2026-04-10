using Contratos;
using Dominio;

namespace Mapeo;

public static class UserMapper
{
    public static Result<User> MappingDTOToInternUser(
        this SingUpInternUserRequestDTO requestDto,
        Phone phone,
        UserRoles role)
    {
        return User.MakeIntern(
            requestDto.Name,
            requestDto.LastName,
            requestDto.Email,
            phone,
            requestDto.Password,
            role);
    }

    public static UsuarioResponseDTO MappingUserToResponseDTO(this User user)
    {
        if(user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
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
