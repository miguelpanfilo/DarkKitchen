using Dominio.Excepciones;

namespace Dominio;

public sealed class Clave
{
    private const int LargoMinimo = 15;
    private const int LargoMaximo = 25;

    public string Valor { get; private set; }

    private Clave(string valor)
    {
        Valor = valor;
    }

    public static bool Validar(string? clave)
    {
        if (string.IsNullOrWhiteSpace(clave))
        {
            return false;
        }

        if (clave.Length < LargoMinimo || clave.Length > LargoMaximo)
        {
            return false;
        }

        if (!clave.Any(char.IsUpper))
        {
            return false;
        }

        if (!clave.Any(char.IsLower))
        {
            return false;
        }

        if (!clave.Any(char.IsDigit))
        {
            return false;
        }

        if (!clave.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            return false;
        }

        if (TieneSecuenciaDeNumeros(clave))
        {
            return false;
        }

        return true;
    }

    public static Clave Crear(string? clave)
    {
        if (!Validar(clave))
        {
            throw new UsuarioExcepcion("La clave no tiene el formato válido");
        }

        return new Clave(clave!);
    }

    private static bool TieneSecuenciaDeNumeros(string clave)
    {
        for (var i = 0; i < clave.Length - 1; i++)
        {
            if (char.IsDigit(clave[i]) && char.IsDigit(clave[i + 1]))
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return Valor;
    }
}
