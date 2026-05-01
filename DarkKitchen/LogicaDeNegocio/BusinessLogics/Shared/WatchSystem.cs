using BusinessLogicsAbstractions.Users;

namespace BusinessLogics.Shared;

public class WatchSystem : IWatch
{
    public DateTime Now()
    {
        return DateTime.Now;
    }
}
