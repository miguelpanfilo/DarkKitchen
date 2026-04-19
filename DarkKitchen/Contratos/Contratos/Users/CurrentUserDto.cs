namespace Contratos.Users;

public class CurrentUserDto
{
    public int Id { get; set; }
    public UserRolesDto Role { get; set; }
    public string Email { get; set; } = string.Empty;
}
