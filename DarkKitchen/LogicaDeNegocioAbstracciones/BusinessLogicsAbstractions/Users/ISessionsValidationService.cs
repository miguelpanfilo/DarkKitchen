using Contract.Users;

namespace BusinessLogicsAbstractions.Users;

public interface ISessionsValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
