namespace Dominio;

public class Session
{
    public Guid SessionId { get; private set; }
    public int UserId { get; private set; }
    public DateTime ExpireDate { get; private set; }

    public Session(int userId, DateTime expireDate)
    {
        SessionId = Guid.NewGuid();
        UserId = userId;
        ExpireDate = expireDate;
    }
}
