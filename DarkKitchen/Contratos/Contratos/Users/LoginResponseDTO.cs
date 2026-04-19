namespace Contratos.Users;

public class LoginResponseDTO
{
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
}
