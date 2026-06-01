namespace BusinessLogicsAbstractions;

public class ForbiddenException(string message) : LogicException(message)
{
}
