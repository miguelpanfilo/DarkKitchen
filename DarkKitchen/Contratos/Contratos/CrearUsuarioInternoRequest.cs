namespace Contratos;

public class CrearUsuarioInternoRequest
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Clave { get; set; } = null!;
    public string Rol { get; set; } = null!;
}
