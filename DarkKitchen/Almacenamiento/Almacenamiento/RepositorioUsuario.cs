using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class RepositorioUsuario : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public Usuario Add(Usuario usuario)
    {
        _usuarios.Add(usuario);
        return usuario;
    }

    public List<Usuario> GetAll()
    {
        return _usuarios;
    }
}
