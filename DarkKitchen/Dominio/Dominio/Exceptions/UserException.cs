namespace Dominio.Exceptions;

public class UserException(string message)
    : Exception(message)
{
}

public class UserValidationException(string message)
    : UserException(message)
{
}

public class UserConflictException(string message)
    : UserException(message)
{
}

public class UserNotFoundException(string message)
    : UserException(message)
{
}

public class UserForbiddenException(string message)
    : UserException(message)
{
}
