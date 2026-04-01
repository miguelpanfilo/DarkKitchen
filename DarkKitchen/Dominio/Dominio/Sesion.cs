namespace Dominio;

public class Sesion
{
    public Guid SessionId { get; private set; }
    public int UsuarioId { get; private set; }
    public DateTime Expiracion { get; private set; }

    public Sesion(int usuarioId, DateTime expiracion)
    {
        SessionId = Guid.NewGuid();
        UsuarioId = usuarioId;
        Expiracion = expiracion;
    }
}
