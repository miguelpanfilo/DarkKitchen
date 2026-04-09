namespace Dominio;

public sealed class Email
{
    public string Value { get; private set; } = null!;

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> TryMakeEmail(string? email)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            return Result<Email>.Fail("Email cannot be empty");
        }

        var emailNormalized = email.Trim().ToLowerInvariant();

        if(!HaveCorrectFormat(emailNormalized))
        {
            return Result<Email>.Fail("Email format is not valid");
        }

        return Result<Email>.Ok(new Email(emailNormalized));
    }

    public override string ToString()
    {
        return Value;
    }

    private static bool HaveCorrectFormat(string mail)
    {
        if(string.IsNullOrWhiteSpace(mail))
        {
            return false;
        }

        if(mail.Contains(' ') || mail.Contains(".."))
        {
            return false;
        }

        var indexOfSymbol = mail.IndexOf('@');

        if(indexOfSymbol <= 0 || indexOfSymbol != mail.LastIndexOf('@'))
        {
            return false;
        }

        var domain = mail[(indexOfSymbol + 1)..];

        if(domain.Length == 0)
        {
            return false;
        }

        if(!domain.Contains('.'))
        {
            return false;
        }

        if(domain.StartsWith('.') || domain.EndsWith('.'))
        {
            return false;
        }

        return true;
    }

    public bool IsTheSame(Email mail)
    {
        return Value == mail.Value;
    }
}
