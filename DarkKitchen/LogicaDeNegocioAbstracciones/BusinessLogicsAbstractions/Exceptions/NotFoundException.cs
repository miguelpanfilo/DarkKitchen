namespace BusinessLogicsAbstractions.Exceptions;

public class NotFoundException(string message) : LogicException(message)
{
}
