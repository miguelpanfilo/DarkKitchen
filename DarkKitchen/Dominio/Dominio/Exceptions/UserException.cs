namespace Dominio.Exceptions;

public class UserException : Exception
{
    public UserException(string message)
        : base(message) { }
}

public class UserValidationException : UserException
{
    public UserValidationException(string message)
        : base(message) { }
}

public class UserConflictException : UserException
{
    public UserConflictException(string message)
        : base(message) { }
}

public class UserNotFoundException : UserException
{
    public UserNotFoundException(string message)
        : base(message) { }
}

public class UserForbiddenException : UserException
{
    public UserForbiddenException(string message)
        : base(message) { }
}
