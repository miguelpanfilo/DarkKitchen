namespace Dominio;

public class Session(int userId, DateTime expireDate)
{
    public Guid SessionId { get; private set; } = Guid.NewGuid();
    public int UserId { get; private set; } = userId;
    public DateTime ExpireDate { get; private set; } = expireDate;
}
