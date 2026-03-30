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
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
        {
            throw new UsuarioExcepcion("El nombre ni el apellido pueden estar vacío");
        }

        if (apellido.Length > 25 || apellido.Length<3)
        {
            throw new UsuarioExcepcion("El apellido debe tener entre 3 y 25 caracteres");
        }

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
        Clave = clave;
    }
}
