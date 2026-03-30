using Dominio.Excepciones;

namespace Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public Usuario(string nombre, string apellido, string email, string telefono, string clave)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new UsuarioExcepcion("El nombre no puede estar vacío");
        }

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
        Clave = clave;
    }
}
