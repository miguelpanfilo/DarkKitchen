using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IRepositorioUsuario
{
    Usuario Add(Usuario usuario);
    List<Usuario> GetAll();
    bool ExisteEmail(string email);
    Usuario? ObtenerPorEmail(string email);
}
