using Dominio.Excepciones;

namespace Dominio;

public sealed class Email
{
    public string Value { get; private set; } = null!;

    private Email(string value)
    {
        Value = value;
    }

    public static Email MakeEmail(string? email)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            throw new UserException("Email cannot be empty");
        }

        var emailNormalized = email.Trim().ToLowerInvariant();

        if(!HaveCorrectFormat(emailNormalized))
        {
            throw new UserException("Email format is not valid");
        }

        return new Email(emailNormalized);
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

    public bool IsTheSame(Email otroEmail)
    {
        return Value == otroEmail.Value;
    }
}
