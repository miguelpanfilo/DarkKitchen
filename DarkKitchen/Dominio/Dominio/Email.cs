using System.ComponentModel.DataAnnotations;
using Dominio.Excepciones;

namespace Dominio;

public sealed class Email
{
    private Email() { }

    public string Valor { get; private set; } = null!;

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Email Crear(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new UsuarioExcepcion("El email no puede estar vacío");
        }

        var emailNormalizado = email.Trim().ToLowerInvariant();

        if (!EsFormatoValido(emailNormalizado))
        {
            throw new UsuarioExcepcion("El email debe tener un formato válido");
        }

        return new Email(emailNormalizado);
    }

    public override string ToString()
    {
        return Valor;
    }

    private static bool EsFormatoValido(string correo)
    {
        var atributo = new EmailAddressAttribute();

        if (!atributo.IsValid(correo))
        {
            return false;
        }

        var indiceArroba = correo.IndexOf('@');
        var dominio = correo[(indiceArroba + 1)..];

        if (!dominio.Contains('.'))
        {
            return false;
        }

        if (dominio.StartsWith('.') || dominio.EndsWith('.'))
        {
            return false;
        }

        if (correo.Contains(".."))
        {
            return false;
        }

        return true;
    }

    public bool Igual(Email otroEmail)
    {
        return Valor == otroEmail.Valor;
    }
}
