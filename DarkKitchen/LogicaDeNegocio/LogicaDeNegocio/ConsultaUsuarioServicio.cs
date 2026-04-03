
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class ConsultaUsuarioServicio : IConsultaUsuarioService
{
    private readonly IRepositorioUsuario _repo;

    public ConsultaUsuarioServicio(IRepositorioUsuario repo)
    {
        _repo = repo;
    }

    public List<UsuarioResponse> BuscarUsuarios(int idUsuarioLogueado, BuscarUsuariosRequest request)
    {
        var usuarioLogueado = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuarioLogueado.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("Solo un usuario administrativo puede realizar esta acción");
        }

        var usuarios = _repo.GetAll();
        var resultado = new List<UsuarioResponse>();

        foreach (var u in usuarios)
        {
            if (CoincideNombre(u, request.Nombre) && CoincideApellido(u, request.Apellido))
            {
                resultado.Add(u.MapeoAResponseDTO());
            }
        }

        return resultado;
    }

    private Usuario ObtenerUsuarioExistente(int id)
    {
        var usuario = _repo.FindById(id);

        if (usuario == null)
        {
            throw new UsuarioExcepcion("El usuario no existe");
        }

        return usuario;
    }

    private static bool CoincideNombre(Usuario usuario, string? nombre)
    {
        return string.IsNullOrWhiteSpace(nombre) ||
               string.Equals(usuario.Nombre, nombre.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool CoincideApellido(Usuario usuario, string? apellido)
    {
        return string.IsNullOrWhiteSpace(apellido) ||
               string.Equals(usuario.Apellido, apellido.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
