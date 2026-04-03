using Dominio.Excepciones;

namespace Dominio;

public class Telefono
{
    public string Valor { get; }

    private Telefono(string valor)
    {
        Valor = valor;
    }

    public static Telefono Crear(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new UsuarioExcepcion("El teléfono no puede estar vacío");
        }

        var telefonoNormalizado = telefono.Trim();

        if (!telefonoNormalizado.All(char.IsDigit))
        {
            throw new UsuarioExcepcion("El teléfono solo puede contener dígitos");
        }

        return new Telefono(telefonoNormalizado);
    }

    public override string ToString()
    {
        return Valor;
    }
}
