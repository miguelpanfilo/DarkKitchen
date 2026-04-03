using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class ModificacionUsuarioService : IModificacionUsuarioService
{
    private readonly IRepositorioUsuario _repo;
    private readonly IValidadorTelefono _validadorTelefono;

    public ModificacionUsuarioService(IRepositorioUsuario repo, IValidadorTelefono validadorTelefono)
    {
        _repo = repo;
        _validadorTelefono = validadorTelefono;
    }

    public UsuarioResponse ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequest request)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);
        ValidarNoAutoModificacion(idUsuarioAModificar, idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAModificar);

        ModificarDatosDelUsuario(usuario, request);

        return usuario.MapeoAResponseDTO();
    }

    public void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequest request)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAModificar);

        if (!usuario.VerificarClave(request.ClaveActual))
        {
            throw new UsuarioExcepcion("La clave actual no es correcta");
        }

        usuario.CambiarClave(Clave.Crear(request.ClaveNueva));
    }

    private void ModificarDatosDelUsuario(Usuario usuario, ModificarUsuarioRequest request)
    {
        var nombreActualizado = CalcularNombreFinal(request, usuario);
        var apellidoActualizado = CalcularApellidoFinal(request, usuario);
        var telefonoActualizado = CalcularTelefonoFinal(request, usuario);
        var emailActualizado = CalcularEmailFinal(request, usuario);

        var telefonoFinal = CrearTelefonoFinal(usuario, telefonoActualizado);
        var rolFinal = CalcularRolFinal(request, usuario);
        ValidarEmailSiCambia(usuario, emailActualizado);

        usuario.ModificarDatos(nombreActualizado, apellidoActualizado, telefonoFinal, emailActualizado, rolFinal);
    }

    private Telefono CrearTelefonoFinal(Usuario usuario, string telefonoActualizado)
    {
        if (telefonoActualizado == usuario.Telefono.Valor)
        {
            return usuario.Telefono;
        }

        return CrearYValidarTelefono(telefonoActualizado);
    }

    private void ValidarEmailSiCambia(Usuario usuario, string emailActualizado)
    {
        if (!string.Equals(emailActualizado, usuario.Email.ToString(), StringComparison.OrdinalIgnoreCase)
            && _repo.ExisteEmail(emailActualizado))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }
    }

    private static string CalcularEmailFinal(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Email)
            ? usuario.Email.ToString()
            : request.Email;
    }

    private static string CalcularApellidoFinal(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Apellido)
            ? usuario.Apellido
            : request.Apellido;
    }

    private static string CalcularNombreFinal(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Nombre)
            ? usuario.Nombre
            : request.Nombre;
    }

    private static string CalcularTelefonoFinal(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Telefono)
            ? usuario.Telefono.Valor
            : request.Telefono;
    }

    private void ValidarUsuarioEsAdministrativo(int idUsuarioLogueado)
    {
        var usuario = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuario.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("Solo un usuario administrativo puede realizar esta acción");
        }
    }

    private void ValidarNoAutoModificacion(int idUsuarioAModificar, int idUsuarioLogueado)
    {
        if (idUsuarioAModificar == idUsuarioLogueado)
        {
            throw new UsuarioExcepcion("Un usuario no puede modificarse a sí mismo");
        }
    }

    private Usuario ObtenerUsuarioExistente(int id)
    {
        var usuario = _repo.FindById(id);

        if (usuario == null)
        {
            throw new UsuarioExcepcion("El usuario no existe");
        }

        return usuario;
    }

    private Telefono CrearYValidarTelefono(string telefono)
    {
        var telefonoCreado = Telefono.Crear(telefono);

        if (!_validadorTelefono.EsValido(telefonoCreado.Valor))
        {
            throw new UsuarioExcepcion("El teléfono no es válido");
        }

        return telefonoCreado;
    }

    private RolUsuario CalcularRolFinal(ModificarUsuarioRequest request, Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(request.Rol))
        {
            return usuario.Rol;
        }

        if (!Enum.TryParse<RolUsuario>(request.Rol, true, out var rol))
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        return rol;
    }
}
