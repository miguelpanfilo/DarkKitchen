using BusinessLogicsAbstractions;
using Contract;
using DataAccessAbstractions;
using Domain;

namespace BusinessLogics;

public class LoginService(
    IRepositoryUser repoUsuario,
    IRepositorySession repoSesion,
    IWatch iWatch) : ILoginService
{
    private readonly IRepositoryUser _repoUsuario = repoUsuario;
    private readonly IRepositorySession _repoSesion = repoSesion;
    private readonly IWatch _iWatch = iWatch;

    public LoginResponseDTO Login(LoginRequestDTO requestDto)
    {
        var userFromTheRequest = _repoUsuario.FindByEmail(requestDto.Email);

        if(userFromTheRequest == null)
        {
            throw new UserAuthenticationException("Credenciales inválidas");
        }

        if(!userFromTheRequest.Password.VerifyHash(requestDto.Password))
        {
            throw new UserAuthenticationException("Credenciales inválidas");
        }

        var expireDate = _iWatch.Now().AddHours(2);
        _repoSesion.RemoveAll();
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
