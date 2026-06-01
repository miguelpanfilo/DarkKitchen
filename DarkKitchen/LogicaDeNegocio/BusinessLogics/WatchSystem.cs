using BusinessLogicsAbstractions;

namespace BusinessLogics;

public class WatchSystem : IWatch
{
    public DateTime Now()
    {
        var uruguayZone = TimeZoneInfo.FindSystemTimeZoneById("America/Montevideo");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, uruguayZone);
    }
}
