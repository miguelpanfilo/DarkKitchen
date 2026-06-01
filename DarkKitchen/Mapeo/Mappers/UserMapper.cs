using Contract;
using Domain;

namespace Mappers;

public static class UserMapper
{
    public static Result<User> MappingDTOToInternUser(
        this SignUpInternUserRequestDTO requestDto,
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

    public static UserResponseDTO MappingUserToResponseDTO(this User user)
    {
        if(user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        return new UserResponseDTO
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email.ToString(),
            Role = user.Role.ToString(),
            Phone = user.Phone.Value
        };
    }

    public static CurrentUserDto ToCurrentUserDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new CurrentUserDto
        {
            Id = user.Id,
            Role = MapRole(user.Role),
            Email = user.Email.ToString()
        };
    }

    private static UserRolesDto MapRole(UserRoles role)
    {
        return role switch
        {
            UserRoles.Client => UserRolesDto.Client,
            UserRoles.Preparer => UserRolesDto.Preparer,
            UserRoles.Administrative => UserRolesDto.Administrative,
            _ => throw new ArgumentOutOfRangeException(nameof(role), "Rol no soportado")
        };
    }
}
