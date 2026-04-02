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

        usuario.ModificarNombreYApellido(nombreActualizado, apellidoActualizado);
    }

    private static string ObtenerNombreActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Nombre)
            ? usuario.Nombre
            : request.Nombre;
    }

    private static string ObtenerApellidoActualizado(ModificarUsuarioRequest request, Usuario usuario)
    {
        return string.IsNullOrWhiteSpace(request.Apellido)
            ? usuario.Apellido
            : request.Apellido;
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
}
