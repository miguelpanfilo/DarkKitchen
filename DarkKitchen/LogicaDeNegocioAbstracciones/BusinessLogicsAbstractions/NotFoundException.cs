namespace BusinessLogicsAbstractions;

public class NotFoundException(string message) : LogicException(message)
{
}
