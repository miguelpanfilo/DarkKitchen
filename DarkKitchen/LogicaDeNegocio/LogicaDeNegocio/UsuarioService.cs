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
        if(!_validadorTelefono.EsValido(request.Telefono))
        {
            throw new UsuarioExcepcion("El teléfono no es válido");
        }

        var usuario = Usuario.CrearCliente(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave
        );

        if(_repo.ExisteEmail(usuario.Email.ToString()))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }

        var usuarioGuardado = _repo.Add(usuario);

        return new RegistrarClienteResponseDTO
        {
            Nombre = usuarioGuardado.Nombre,
            Apellido = usuarioGuardado.Apellido,
            Email = usuarioGuardado.Email.ToString()
        };
    }

    public UsuarioResponse CrearUsuarioInterno(CrearUsuarioInternoRequest request)
    {
        if(!_validadorTelefono.EsValido(request.Telefono))
        {
            throw new UsuarioExcepcion("El teléfono no es válido");
        }

        if(_repo.ExisteEmail(request.Email))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }

        if(!Enum.TryParse<RolUsuario>(request.Rol, true, out var rol))
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        var usuario = Usuario.CrearInterno(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave,
            rol
        );

        var usuarioGuardado = _repo.Add(usuario);

        return usuarioGuardado.MapeoAResponseDTO();
    }

    public UsuarioResponse ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequest request)
    {
        ValidarNoAutoModificacion(idUsuarioAModificar, idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAModificar);

        ModificarDatosDelUsuario(usuario, request);

        return usuario.MapeoAResponseDTO();
    }

    private void ModificarDatosDelUsuario(Usuario usuario, ModificarUsuarioRequest request)
    {
        var nombreActualizado = ObtenerNombreActualizado(request, usuario);
        var apellidoActualizado = ObtenerApellidoActualizado(request, usuario);
        var telefonoActualizado = ObtenerTelefonoActualizado(request, usuario);
        var emailActualizado = ObtenerEmailActualizado(request, usuario);

        ValidarTelefonoSiCambia(usuario, telefonoActualizado);
        ValidarEmailSiCambia(usuario, emailActualizado);

        usuario.ModificarDatos(nombreActualizado, apellidoActualizado, telefonoActualizado, emailActualizado);
    }

    private void ValidarTelefonoSiCambia(Usuario usuario, string telefonoActualizado)
    {
        if (telefonoActualizado != usuario.Telefono && !_validadorTelefono.EsValido(telefonoActualizado))
        {
            throw new UsuarioExcepcion("El teléfono no es válido");
        }
    }

    private void ValidarEmailSiCambia(Usuario usuario, string emailActualizado)
    {
        if (!string.Equals(emailActualizado, usuario.Email.ToString(), StringComparison.OrdinalIgnoreCase)
            && _repo.ExisteEmail(emailActualizado))
        {
            throw new UsuarioExcepcion("El email ya está registrado");
        }
    }

    private static string ObtenerEmailActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Email)
            ? usuario.Email.ToString()
            : request.Email;
    }

    private static string ObtenerApellidoActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Apellido)
            ? usuario.Apellido
            : request.Apellido;
    }

    private static string ObtenerNombreActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Nombre)
            ? usuario.Nombre
            : request.Nombre;
    }

    private static string ObtenerTelefonoActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Telefono)
            ? usuario.Telefono
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

    public void ModificarClaveUsuario(int idUsuario, ModificarClaveRequest request)
    {
        var usuario = ObtenerUsuarioExistente(idUsuario);

        if (!usuario.VerificarClave(request.ClaveActual))
        {
            throw new UsuarioExcepcion("La clave actual no es correcta");
        }

        usuario.CambiarClave(Clave.Crear(request.ClaveNueva));
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        var usuarioLogueado = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuarioLogueado.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("No tiene permisos para eliminar usuarios");
        }

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

    public List<UsuarioResponse> BuscarUsuariosPorNombre(string nombre)
    {
        var nombreEnMinuscula=nombre.ToLower();
        return _repo.GetAll()
            .Where(u => string.Equals(u.Nombre.ToLower(), nombreEnMinuscula))
            .Select(u => u.MapeoAResponseDTO())
            .ToList();
    }

    public List<UsuarioResponse> BuscarUsuariosPorApellido(string apellido)
    {
        var apellidoNormalizado = apellido.Trim();

        var usuarios = _repo.GetAll();
        var resultado = new List<UsuarioResponse>();

        foreach (var u in usuarios)
        {
            if (string.Equals(u.Apellido, apellidoNormalizado, StringComparison.OrdinalIgnoreCase))
            {
                resultado.Add(u.MapeoAResponseDTO());
            }
        }

        return resultado;
    }
}
