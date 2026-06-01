using Contract.Users;

namespace BusinessLogicsAbstractions;

public interface ISessionsValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
