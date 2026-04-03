using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class EliminacionUsuarioService : IEliminacionUsuarioService
{
    private readonly IRepositorioUsuario _repo;

    public EliminacionUsuarioService(IRepositorioUsuario repo)
    {
        _repo = repo;
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        ValidarUsuarioEsAdministrativo(idUsuarioLogueado);
        ValidarNoAutoEliminacion(idUsuarioAEliminar, idUsuarioLogueado);

        var usuario = ObtenerUsuarioExistente(idUsuarioAEliminar);

        _repo.Remove(usuario.Id);
    }

    private void ValidarUsuarioEsAdministrativo(int idUsuarioLogueado)
    {
        var usuario = ObtenerUsuarioExistente(idUsuarioLogueado);

        if (usuario.Rol != RolUsuario.Administrativo)
        {
            throw new UsuarioExcepcion("Solo un usuario administrativo puede realizar esta acción");
        }
    }

    private void ValidarNoAutoEliminacion(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        if (idUsuarioAEliminar == idUsuarioLogueado)
        {
            throw new UsuarioExcepcion("Un usuario no puede eliminarse a sí mismo");
        }
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
}
