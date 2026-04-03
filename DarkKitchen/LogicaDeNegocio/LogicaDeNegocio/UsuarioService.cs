using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class UsuarioService
{
    private readonly IRepositorioUsuario _repo;
    private readonly IValidadorTelefono _validadorTelefono;

    public UsuarioService(IRepositorioUsuario repo, IValidadorTelefono validadorTelefono)
    {
        _repo = repo;
        _validadorTelefono = validadorTelefono;
    }

    public RegistrarClienteResponseDTO RegistrarCliente(RegistrarClienteRequest request)
    {
        if(_repo.ExisteEmail(request.Email))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }

        var telefono = CrearYValidarTelefono(request.Telefono);

        var usuario = Usuario.CrearCliente(
            request.Nombre,
            request.Apellido,
            request.Email,
            telefono,
            request.Clave
        );

        var usuarioGuardado = _repo.Add(usuario);

        return new RegistrarClienteResponseDTO
        {
            Nombre = usuarioGuardado.Nombre,
            Apellido = usuarioGuardado.Apellido,
            Email = usuarioGuardado.Email.ToString()
        };
    }

    private RolUsuario ObtenerRolInternoValido(string rol)
    {
        if (!Enum.TryParse<RolUsuario>(rol, true, out var rolEnum))
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        return rolEnum;
    }

    public UsuarioResponse CrearUsuarioInterno(int idUsuarioLogueado, CrearUsuarioInternoRequest request)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);

        var rol = ObtenerRolInternoValido(request.Rol);
        var telefono = CrearYValidarTelefono(request.Telefono);

        if (_repo.ExisteEmail(request.Email))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }

        var usuario = request.MapeoDTOAUserInterno(telefono, rol);

        var usuarioGuardado = _repo.Add(usuario);

        return usuarioGuardado.MapeoAResponseDTO();
    }

    private void ValidarUsuarioEsAdministrativo(int idUsuarioLogueado)
    {
        var usuario = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuario.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("Solo un usuario administrativo puede realizar esta acción");
        }
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

    private void ModificarDatosDelUsuario(Usuario usuario, ModificarUsuarioRequest request)
    {
        var nombreActualizado = CalcularNombreFinal(request, usuario);
        var apellidoActualizado = CalcularApellidoFinal(request, usuario);
        var telefonoActualizado = CalcularTelefonoFinal(request, usuario);
        var emailActualizado = CalcularEmailFinal(request, usuario);

        var telefonoFinal = CrearTelefonoFinal(usuario, telefonoActualizado);
        ValidarEmailSiCambia(usuario, emailActualizado);

        usuario.ModificarDatos(nombreActualizado, apellidoActualizado, telefonoFinal, emailActualizado);
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

    private void ValidarNoAutoModificacion(int idUsuarioAModificar, int idUsuarioLogueado)
    {
        if(idUsuarioAModificar == idUsuarioLogueado)
        {
            throw new UsuarioExcepcion("Un usuario no puede modificarse a sí mismo");
        }
    }

    private Usuario ObtenerUsuarioExistente(int id)
    {
        var usuario = _repo.FindById(id);

        if(usuario == null)
        {
            throw new UsuarioExcepcion("El usuario no existe");
        }

        return usuario;
    }

    public void ModificarClaveUsuario(int idUsuarioAModificar, int idUsuarioLogueado, ModificarClaveRequest request)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAModificar);

        if (!usuario.VerificarClave(request.ClaveActual))
        {
            throw new UsuarioExcepcion("La clave actual no es correcta");
        }

        usuario.CambiarClave(Clave.Crear(request.ClaveNueva));
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);
        ValidarNoAutoEliminacion(idUsuarioAEliminar, idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAEliminar);

        _repo.Remove(usuario.Id);
    }

    private void ValidarNoAutoEliminacion(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        if (idUsuarioAEliminar == idUsuarioLogueado)
        {
            throw new UsuarioExcepcion("Un usuario no puede eliminarse a sí mismo");
        }
    }

    public List<UsuarioResponse> BuscarUsuarios(int idUsuarioLogueado, BuscarUsuariosRequest request)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);

        var usuarios = _repo.GetAll();
        var resultado = new List<UsuarioResponse>();

        foreach (var u in usuarios)
        {
            if (CoincideNombre(u, request.Nombre) && CoincideApellido(u, request.Apellido))
            {
                resultado.Add(u.MapeoAResponseDTO());
            }
        }

        return resultado;
    }

    private static bool CoincideNombre(Usuario usuario, string? nombre)
    {
        return string.IsNullOrWhiteSpace(nombre) ||
               string.Equals(usuario.Nombre, nombre.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool CoincideApellido(Usuario usuario, string? apellido)
    {
        return string.IsNullOrWhiteSpace(apellido) ||
               string.Equals(usuario.Apellido, apellido.Trim(), StringComparison.OrdinalIgnoreCase);
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
}
