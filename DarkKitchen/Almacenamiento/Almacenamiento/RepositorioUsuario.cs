using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class RepositorioUsuario : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    private int _ultimoId = 0;

    public Usuario Add(Usuario usuario)
    {
        _ultimoId++;
        usuario.Id = _ultimoId;
        _usuarios.Add(usuario);
        return usuario;
    }

    public List<Usuario> GetAll()
    {
        return _usuarios;
    }

    public bool ExisteEmail(string email)
    {
        var emailBuscado = Email.Crear(email);

        return _usuarios.Any(u => u.Email.Igual(emailBuscado));
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        var emailBuscado = Email.Crear(email);

        return _usuarios.FirstOrDefault(u => u.Email.Igual(emailBuscado));
    }

    public Usuario? FindById(int id)
    {
        return _usuarios.FirstOrDefault(u => u.Id == id);
    }
}
