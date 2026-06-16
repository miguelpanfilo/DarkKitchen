using Contract;

namespace BusinessLogicsAbstractions;

public interface ISessionsValidationService
{
    CurrentUserDto ValidateSession(Guid sessionId);
}
