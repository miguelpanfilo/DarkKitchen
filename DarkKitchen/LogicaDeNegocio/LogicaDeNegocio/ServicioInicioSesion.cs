using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class ServicioInicioSesion
{
    private readonly IRepositorioUsuario _repoUsuario;
    private readonly IRepositorioSesion _repoSesion;
    private readonly IReloj _reloj;

    public ServicioInicioSesion(
        IRepositorioUsuario repoUsuario,
        IRepositorioSesion repoSesion,
        IReloj reloj)
    {
        _repoUsuario = repoUsuario;
        _repoSesion = repoSesion;
        _reloj = reloj;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var usuarioMailIngresado = _repoUsuario.ObtenerPorEmail(request.Email);

        if (usuarioMailIngresado == null)
        {
            throw new UsuarioExcepcion("Credenciales inválidas");
        }

        if (!usuarioMailIngresado.Clave.Verificar(request.Clave))
        {
            throw new UsuarioExcepcion("Credenciales inválidas");
        }

        var expiracion = _reloj.Ahora().AddHours(2);
        var sesion = new Sesion(usuarioMailIngresado.Id, expiracion);
        var sesionGuardada = _repoSesion.Add(sesion);

        return new LoginResponse
        {
            SessionId = sesionGuardada.SessionId,
            UsuarioId = usuarioMailIngresado.Id,
            Rol = usuarioMailIngresado.Rol.ToString(),
            Expiracion = sesionGuardada.Expiracion
        };
    }
}
