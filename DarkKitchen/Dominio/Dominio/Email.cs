namespace Dominio;

public sealed class Email
{
    private const char AtSymbol = '@';
    private const char Dot = '.';
    private const string DoubleDot = "..";

    private const string EmptyEmailMessage = "Email no puede estar vacío";
    private const string InvalidFormatMessage = "Formato de Email inválido";

    public string Value { get; private set; } = null!;

    private Email(string value)
    {
        Value = value;
    }

    private Email()
    {
        Value = null!;
    }

    public static Result<Email> TryMakeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Email>.Fail(EmptyEmailMessage);
        }

        var normalized = email.Trim().ToLowerInvariant();

        if (!HasValidFormat(normalized))
        {
            return Result<Email>.Fail(InvalidFormatMessage);
        }

        return Result<Email>.Ok(new Email(normalized));
    }

    private static bool HasValidFormat(string mail)
    {
        if (mail.Contains(' ') || mail.Contains(DoubleDot))
        {
            return false;
        }

        var atIndex = mail.IndexOf(AtSymbol);

        if (atIndex <= 0 || atIndex != mail.LastIndexOf(AtSymbol))
        {
            return false;
        }

        var domainStartIndex = atIndex + 1;
        var domain = mail[domainStartIndex..];

        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }

        if (!domain.Contains(Dot))
        {
            return false;
        }

        if (domain.StartsWith(Dot) || domain.EndsWith(Dot))
        {
            return false;
        }

        return true;
    }

    public bool IsTheSame(Email mail)
    {
        return Value == mail.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
