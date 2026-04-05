using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class LoginService(
    IRepositoryUser repoUsuario,
    IRepositorySession repoSesion,
    IReloj reloj)
{
    private readonly IRepositoryUser _repoUsuario = repoUsuario;
    private readonly IRepositorySession _repoSesion = repoSesion;
    private readonly IReloj _reloj = reloj;

    public LoginResponseDTO Login(LoginRequestDTO requestDto)
    {
        var usuarioMailIngresado = _repoUsuario.FindByEmail(requestDto.Email);

        if(usuarioMailIngresado == null)
        {
            throw new UserException("Invalid credentials");
        }

        if(!usuarioMailIngresado.Password.VerifyHash(requestDto.Password))
        {
            throw new UserException("Invalid credentials");
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
