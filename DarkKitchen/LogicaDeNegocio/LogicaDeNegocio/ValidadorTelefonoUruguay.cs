using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class ValidadorTelefonoUruguay : IValidadorTelefono
{
    public bool EsValido(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            return false;
        }

        if (telefono.Length != 9)
        {
            return false;
        }

        if (!telefono.All(char.IsDigit))
        {
            return false;
        }

        var prefijosValidos = new[]
        {
            "091", "092", "093", "094",
            "095", "096", "097", "098", "099"
        };

        if (!prefijosValidos.Any(p => telefono.StartsWith(p)))
        {
            return false;
        }

        return true;
    }
}
