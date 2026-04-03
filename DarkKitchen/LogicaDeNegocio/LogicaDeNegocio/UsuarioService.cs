using Contratos;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class UsuarioService
{
    private readonly IAltaUsuarioService _altaUsuarioService;
    private readonly IModificacionUsuarioService _modificacionUsuarioService;
    private readonly IEliminacionUsuarioService _eliminacionUsuarioService;
    private readonly IConsultaUsuarioService _consultaUsuarioService;

    public UsuarioService(
        IAltaUsuarioService altaUsuarioService,
        IModificacionUsuarioService modificacionUsuarioService,
        IEliminacionUsuarioService eliminacionUsuarioService,
        IConsultaUsuarioService consultaUsuarioService)
    {
        _altaUsuarioService = altaUsuarioService;
        _modificacionUsuarioService = modificacionUsuarioService;
        _eliminacionUsuarioService = eliminacionUsuarioService;
        _consultaUsuarioService = consultaUsuarioService;
    }

    public RegistrarClienteResponseDTO RegistrarCliente(RegistrarClienteRequest request)
    {
        return _altaUsuarioService.RegistrarCliente(request);
    }

    public UsuarioResponse CrearUsuarioInterno(int idUsuarioLogueado, CrearUsuarioInternoRequest request)
    {
        return _altaUsuarioService.CrearUsuarioInterno(idUsuarioLogueado, request);
    }

    public UsuarioResponse ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequest request)
    {
        return _modificacionUsuarioService.ModificarUsuarioInterno(
            idUsuarioAModificar,
            idUsuarioLogueado,
            request
        );
    }

    public void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequest request)
    {
        _modificacionUsuarioService.ModificarClaveUsuario(
            idUsuarioAModificar,
            idUsuarioLogueado,
            request
        );
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        _eliminacionUsuarioService.EliminarUsuarioInterno(idUsuarioAEliminar, idUsuarioLogueado);
    }

    public List<UsuarioResponse> BuscarUsuarios(int idUsuarioLogueado, BuscarUsuariosRequest request)
    {
        return _consultaUsuarioService.BuscarUsuarios(idUsuarioLogueado, request);
    }
}
