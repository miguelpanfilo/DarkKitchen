using Dominio.Excepciones;

namespace Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public string Telefono { get; private set; } = string.Empty;
    public Clave Clave { get; private set; }
    public RolUsuario Rol { get; private set; }

    public static Usuario CrearCliente(string nombre, string apellido, string email, string telefono, string clave)
    {
        var emailValido = Email.Crear(email);
        var claveHasheada = Clave.Crear(clave);

        return new Usuario(nombre, apellido, emailValido, telefono, claveHasheada, RolUsuario.Cliente);
    }

    private Usuario(string nombre, string apellido, Email email, string telefono, Clave clave, RolUsuario rol)
    {
        ValidarNombre(nombre);
        ValidarApellido(apellido);
        ValidarTelefonoNoVacio(telefono);

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Email = email;
        Telefono = telefono.Trim();
        Clave = clave;
        Rol = rol;
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

        if (apellido.Trim().Length < 3 || apellido.Trim().Length > 25)
        {
            throw new UsuarioExcepcion("El apellido debe tener entre 3 y 25 caracteres");
        }
    }

    private void ValidarTelefonoNoVacio(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new UsuarioExcepcion("El teléfono no puede estar vacío");
        }
    }

    public static Usuario CrearInterno(
        string nombre,
        string apellido,
        string email,
        string telefono,
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
}
