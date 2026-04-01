using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class RelojSistema : IReloj
{
    public DateTime Ahora()
    {
        return DateTime.Now;
    }
}
