using Dominio.Excepciones;

namespace Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public Email Email { get; private set; }
    public string Telefono { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public static Usuario Crear(string nombre, string apellido, string email, string telefono, string clave)
    {
        var emailValido = Email.Crear(email);
        return new Usuario(nombre, apellido, emailValido, telefono, clave);
    }

    private Usuario(string nombre, string apellido, Email email, string telefono, string clave)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);
        ValidarTelefonoNoVacio(telefono);
        ValidarClave(clave);
        if (!clave.Any(char.IsUpper))
        {
            throw new UsuarioExcepcion("La clave debe tener al menos una mayúscula");
        }

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
        Clave = clave;
    }

    private void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new UsuarioExcepcion("El nombre no puede estar vacío");
        }
    }

    private void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new UsuarioExcepcion("El apellido no puede estar vacío");
        }

        if (apellido.Length < 3 || apellido.Length > 25)
        {
            throw new UsuarioExcepcion("El apellido debe tener entre 3 y 25 caracteres");
        }
    }

    private void ValidarClave(string clave)
    {
        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new UsuarioExcepcion("La clave no puede estar vacía");
        }

        if (clave.Length < 15 || clave.Length > 25)
        {
            throw new UsuarioExcepcion("La clave debe tener entre 15 y 25 caracteres");
        }
    }

    private void ValidarTelefonoNoVacio(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new UsuarioExcepcion("El teléfono no puede estar vacío");
        }
    }
}
