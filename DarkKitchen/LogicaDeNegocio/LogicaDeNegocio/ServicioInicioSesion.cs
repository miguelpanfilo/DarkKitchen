using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;

namespace LogicaDeNegocio;

public class ServicioInicioSesion
{
    private readonly IRepositorioUsuario _repoUsuario;
    private readonly IRepositorioSesion _repoSesion;

    public ServicioInicioSesion(
        IRepositorioUsuario repoUsuario,
        IRepositorioSesion repoSesion)
    {
        _repoUsuario = repoUsuario;
        _repoSesion = repoSesion;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var usuario = _repoUsuario.ObtenerPorEmail(request.Email);

        if (usuario == null)
        {
            throw new UsuarioExcepcion("Credenciales inválidas");
        }

        if (!usuario.Clave.Verificar(request.Clave))
        {
            throw new UsuarioExcepcion("Credenciales inválidas");
        }

        var expiracion = DateTime.Now.AddHours(2);
        var sesion = new Sesion(usuario.Id, expiracion);

        var sesionGuardada = _repoSesion.Add(sesion);

        return new LoginResponse
        {
            SessionId = sesionGuardada.SessionId,
            UsuarioId = usuario.Id,
            Rol = usuario.Rol.ToString(),
            Expiracion = sesionGuardada.Expiracion
        };
    }
}
