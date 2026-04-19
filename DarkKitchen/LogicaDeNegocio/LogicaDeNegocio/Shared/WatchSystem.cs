using LogicaDeNegocioAbstracciones.Users;

namespace LogicaDeNegocio.Shared;

public class WatchSystem : IWatch
{
    public DateTime Now()
    {
        return DateTime.Now;
    }
}
