using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;

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
        if (!_validadorTelefono.EsValido(request.Telefono))
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

        if (_repo.ExisteEmail(usuario.Email.ToString()))
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
}
