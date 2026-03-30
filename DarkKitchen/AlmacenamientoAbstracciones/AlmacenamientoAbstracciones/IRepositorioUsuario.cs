using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IRepositorioUsuario
{
    Usuario Add(Usuario usuario);
    List<Usuario> GetAll();
}
