using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class WatchSystem : IReloj
{
    public DateTime Ahora()
    {
        return DateTime.Now;
    }
}
