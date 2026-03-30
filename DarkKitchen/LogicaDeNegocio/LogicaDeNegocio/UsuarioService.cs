using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;

namespace LogicaDeNegocio;

public class UsuarioService
{
    private readonly IRepositorioUsuario _repo;

    public UsuarioService(IRepositorioUsuario repo)
    {
        _repo = repo;
    }

    public RegistrarClienteResponseDTO RegistrarCliente(RegistrarClienteRequest request)
    {
        var usuario = new Usuario(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave
        );

        var usuarioGuardado = _repo.Add(usuario);

        return new RegistrarClienteResponseDTO
        {
            Nombre = usuarioGuardado.Nombre,
            Apellido = usuarioGuardado.Apellido,
            Email = usuarioGuardado.Email
        };
    }
}
