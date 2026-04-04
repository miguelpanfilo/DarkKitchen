using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IRepositorySession
{
    Session Add(Session session);
}
