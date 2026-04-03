using Dominio.Excepciones;

namespace Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public Telefono Telefono { get; private set; }
    public Clave Clave { get; private set; }
    public RolUsuario Rol { get; private set; }

    public static Usuario CrearCliente(string nombre, string apellido, string email, Telefono telefono, string clave)
    {
        var emailValido = Email.Crear(email);
        var claveHasheada = Clave.Crear(clave);

        return new Usuario(nombre, apellido, emailValido, telefono, claveHasheada, RolUsuario.Cliente);
    }

    private Usuario(string nombre, string apellido, Email email, Telefono telefono, Clave clave, RolUsuario rol)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Email = email;
        Telefono = telefono;
        Clave = clave;
        Rol = rol;
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new UsuarioExcepcion("El nombre no puede estar vacío");
        }

        if (nombre.Any(char.IsDigit))
        {
            throw new UsuarioExcepcion("El nombre no puede contener números");
        }
    }

    private static void ValidarApellido(string apellido)
    {
        if (string.IsNullOrWhiteSpace(apellido))
        {
            throw new UsuarioExcepcion("El apellido no puede estar vacío");
        }

        if (apellido.Trim().Length < 3 || apellido.Trim().Length > 25)
        {
            throw new UsuarioExcepcion("El apellido debe tener entre 3 y 25 caracteres");
        }
    }

    public static Usuario CrearInterno(
        string nombre,
        string apellido,
        string email,
        Telefono telefono,
        string clave,
        RolUsuario rol)
    {
        var esRolValido = rol == RolUsuario.Preparador || rol == RolUsuario.Administrativo;

        if (!esRolValido)
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        var emailValido = Email.Crear(email);
        var claveValida = Clave.Crear(clave);

        return new Usuario(nombre, apellido, emailValido, telefono, claveValida, rol);
    }

    public void ModificarNombre(string nombre)
    {
        ValidarNombre(nombre);
        Nombre = nombre.Trim();
    }

    public void ModificarDatos(string nombre, string apellido, Telefono telefono, string email)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Telefono = telefono;
        Email = Email.Crear(email);
    }

    public bool VerificarClave(string clave)
    {
        return Clave.Verificar(clave);
    }

    public void CambiarClave(Clave nuevaClave)
    {
        Clave = nuevaClave;
    }
}
