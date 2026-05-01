namespace BusinessLogicsAbstractions.Exceptions;

public class ForbiddenException(string message) : LogicException(message)
{
}
