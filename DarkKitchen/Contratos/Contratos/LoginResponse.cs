namespace Contratos;

public class LoginResponse
{
    public Guid SessionId { get; set; }
    public int UsuarioId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}
