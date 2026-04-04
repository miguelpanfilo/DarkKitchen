using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class LoginService
{
    private readonly IRepositoryUser _repoUsuario;
    private readonly IRepositorySession _repoSesion;
    private readonly IReloj _reloj;

    public LoginService(
        IRepositoryUser repoUsuario,
        IRepositorySession repoSesion,
        IReloj reloj)
    {
        _repoUsuario = repoUsuario;
        _repoSesion = repoSesion;
        _reloj = reloj;
    }

    public LoginResponseDTO Login(LoginRequestDTO requestDto)
    {
        var usuarioMailIngresado = _repoUsuario.FindByEmail(requestDto.Email);

        if (usuarioMailIngresado == null)
        {
            throw new UsuarioExcepcion("Invalid credentials");
        }

        if (!usuarioMailIngresado.Password.VerifyHash(requestDto.Password))
        {
            throw new UsuarioExcepcion("Invalid credentials");
        }

        var expiracion = _reloj.Ahora().AddHours(2);
        var sesion = new Session(usuarioMailIngresado.Id, expiracion);
        var sesionGuardada = _repoSesion.Add(sesion);

        return new LoginResponseDTO
        {
            SessionId = sesionGuardada.SessionId,
            UserId = usuarioMailIngresado.Id,
            Role = usuarioMailIngresado.Role.ToString(),
            ExpireDate = sesionGuardada.ExpireDate
        };
    }
}
