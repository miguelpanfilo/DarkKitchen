using System.Security.Cryptography;
using System.Text;
using Dominio.Excepciones;

namespace Dominio;

public sealed class Clave
{
    private const int LargoMinimo = 15;
    private const int LargoMaximo = 25;

    public string Hash { get; private set; }

    private Clave(string hash)
    {
        Hash = hash;
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

        var hash = HashearMd5(clave!);
        return new Clave(hash);
    }

    private static string HashearMd5(string texto)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(texto);
        var hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash);
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

    public bool Verificar(string clave)
    {
        var hashIngresado = HashearMd5(clave);
        return string.Equals(hashIngresado, Hash);
    }

    public string ObtenerHash()
    {
        return Hash;
    }
}
