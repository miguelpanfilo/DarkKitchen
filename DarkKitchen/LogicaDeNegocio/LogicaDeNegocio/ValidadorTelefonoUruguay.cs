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

        if (!telefono.StartsWith("09"))
        {
            return false;
        }

        return true;
    }
}
