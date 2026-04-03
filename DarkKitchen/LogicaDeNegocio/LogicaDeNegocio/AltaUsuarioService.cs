using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class AltaUsuarioService : IAltaUsuarioService
{
    private readonly IRepositorioUsuario _repo;
    private readonly IValidadorTelefono _validadorTelefono;

    public AltaUsuarioService(IRepositorioUsuario repo, IValidadorTelefono validadorTelefono)
    {
        _repo = repo;
        _validadorTelefono = validadorTelefono;
    }

    public RegistrarClienteResponseDTO RegistrarCliente(RegistrarClienteRequest request)
    {
        if (_repo.ExisteEmail(request.Email))
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

    private RolUsuario ObtenerRolInternoValido(string rol)
    {
        if (!Enum.TryParse<RolUsuario>(rol, true, out var rolEnum))
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        return rolEnum;
    }

    private void ValidarUsuarioEsAdministrativo(int idUsuarioLogueado)
    {
        var usuario = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuario.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("Solo un usuario administrativo puede realizar esta acción");
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
}
