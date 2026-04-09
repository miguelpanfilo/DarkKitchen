using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class LoginService(
    IRepositoryUser repoUsuario,
    IRepositorySession repoSesion,
    IReloj iWatch) : ILoginService
{
    private readonly IRepositoryUser _repoUsuario = repoUsuario;
    private readonly IRepositorySession _repoSesion = repoSesion;
    private readonly IReloj _iWatch = iWatch;

    public LoginResponseDTO Login(LoginRequestDTO requestDto)
    {
        var userFromTheRequest = _repoUsuario.FindByEmail(requestDto.Email);

        if(userFromTheRequest == null)
        {
            throw new UserAuthenticationException("Invalid credentials");
        }

        if(!userFromTheRequest.Password.VerifyHash(requestDto.Password))
        {
            throw new UserAuthenticationException("Invalid credentials");
        }

        var expireDate = _iWatch.Ahora().AddHours(2);
        var session = new Session(userFromTheRequest.Id, expireDate);
        var savedSession = _repoSesion.Add(session);

        return new LoginResponseDTO
        {
            SessionId = savedSession.SessionId,
            UserId = userFromTheRequest.Id,
            Role = userFromTheRequest.Role.ToString(),
            ExpireDate = savedSession.ExpireDate
        };
    }
}
