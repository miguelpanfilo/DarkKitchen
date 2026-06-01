using BusinessLogicsAbstractions;

namespace BusinessLogics;

public static class ServiceExceptionHandler
{
    public static void ExecuteWithValidationHandling<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public static T ExecuteWithValidationHandling<T, TException>(Func<T> action)
        where TException : Exception
    {
        try
        {
            return action();
        }
        catch (TException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public static void ExecuteWithConflictHandling<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
