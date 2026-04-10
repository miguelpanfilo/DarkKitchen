namespace Dominio;

public class Result<T>
{
    public bool Success { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }

    private Result(bool success, T? value, string? errorMessage)
    {
        Success = success;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Fail(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage);
    }
}
