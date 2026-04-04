using System.ComponentModel.DataAnnotations;
using Dominio.Excepciones;

namespace Dominio;

public sealed class Email
{
    private Email() { }

    public string Value { get; private set; } = null!;

    private Email(string value)
    {
        Value = value;
    }

    public static Email MakeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new UsuarioExcepcion("Email cannot be empty");
        }

        var emailNormalized = email.Trim().ToLowerInvariant();

        if (!HaveCorrectFormat(emailNormalized))
        {
            throw new UsuarioExcepcion("Email format is not valid");
        }

        return new Email(emailNormalized);
    }

    public override string ToString()
    {
        return Value;
    }

    private static bool HaveCorrectFormat(string mail)
    {
        var atribute = new EmailAddressAttribute();

        if (!atribute.IsValid(mail))
        {
            return false;
        }

        var indexOfSymbol = mail.IndexOf('@');
        var domain = mail[(indexOfSymbol + 1)..];

        if (!domain.Contains('.'))
        {
            return false;
        }

        if (domain.StartsWith('.') || domain.EndsWith('.'))
        {
            return false;
        }

        if (mail.Contains(".."))
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
