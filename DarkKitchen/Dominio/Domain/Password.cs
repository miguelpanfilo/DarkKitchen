using System.Security.Cryptography;
using System.Text;

namespace Domain;

public sealed class Password
{
    private const int MinLenght = 15;
    private const int MaxLenght = 25;

    public string Hash { get; private set; }

    private Password(string hash)
    {
        Hash = hash;
    }

    private Password()
    {
        Hash = null!;
    }

    public static bool Validate(string? password)
    {
        if(string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if(password.Length < MinLenght || password.Length > MaxLenght)
        {
            return false;
        }

        if(!password.Any(char.IsUpper))
        {
            return false;
        }

        if(!password.Any(char.IsLower))
        {
            return false;
        }

        if(!password.Any(char.IsDigit))
        {
            return false;
        }

        if(!password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            return false;
        }

        if(HaveSequenceOfNumbers(password))
        {
            return false;
        }

        return true;
    }

    public static Result<Password> TryMakePassword(string? password)
    {
        if(!Validate(password))
        {
            return Result<Password>.Fail("Formato de clave inválida");
        }

        var hash = HashearMd5(password!);
        return Result<Password>.Ok(new Password(hash));
    }

    private static string HashearMd5(string texto)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(texto);
        var hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    private static bool HaveSequenceOfNumbers(string password)
    {
        for(var i = 0; i < password.Length - 1; i++)
        {
            if(char.IsDigit(password[i]) && char.IsDigit(password[i + 1]))
            {
                return true;
            }
        }

        return false;
    }

    public bool VerifyHash(string clave)
    {
        var hashIngresado = HashearMd5(clave);
        return string.Equals(hashIngresado, Hash);
    }

    public string GetHash()
    {
        return Hash;
    }
}
