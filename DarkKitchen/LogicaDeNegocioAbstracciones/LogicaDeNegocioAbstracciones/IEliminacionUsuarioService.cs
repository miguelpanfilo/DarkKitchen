using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IEliminacionUsuarioService
{
    void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado);
}
