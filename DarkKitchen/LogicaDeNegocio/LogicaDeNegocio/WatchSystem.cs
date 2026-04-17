using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class WatchSystem : IReloj
{
    public DateTime Now()
    {
        return DateTime.Now;
    }
}
