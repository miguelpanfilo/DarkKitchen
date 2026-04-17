namespace Dominio;

public class Session
{
    public Guid SessionId { get; private set; } = Guid.NewGuid();
    public int UserId { get; private set; }
    public DateTime ExpireDate { get; private set; }

    private Session() { }

    public Session(int userId, DateTime expireDate)
    {
        UserId = userId;
        ExpireDate = expireDate;
    }
}
