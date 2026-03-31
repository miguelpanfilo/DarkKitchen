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
        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new UsuarioExcepcion("La clave no puede estar vacía");
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

    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new UsuarioExcepcion("El email no puede estar vacío");
        }

        if (!email.Contains("@"))
        {
            throw new UsuarioExcepcion("El email debe tener un formato válido");
        }

        if (email.Contains(" "))
        {
            throw new UsuarioExcepcion("El email no debe contener espacios");
        }

        var partes = email.Split('@');

        if (partes.Length != 2 || !partes[1].Contains('.')||string.IsNullOrWhiteSpace(partes[0]))
        {
            throw new UsuarioExcepcion("El email debe tener un formato válido");
        }

        if (partes[1].StartsWith(".") || partes[1].EndsWith("."))
        {
            throw new UsuarioExcepcion("El dominio no es válido");
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
