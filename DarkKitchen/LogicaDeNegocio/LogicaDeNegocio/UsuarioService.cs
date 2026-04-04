using Contratos;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class UsuarioService
{
    private readonly IAltaUsuarioService _altaUsuarioService;
    private readonly IModifyUserService _modifyUserService;
    private readonly IDeleteUserService _deleteUserService;
    private readonly IConsultaUsuarioService _consultaUsuarioService;

    public UsuarioService(
        IAltaUsuarioService altaUsuarioService,
        IModifyUserService modifyUserService,
        IDeleteUserService deleteUserService,
        IConsultaUsuarioService consultaUsuarioService)
    {
        _altaUsuarioService = altaUsuarioService;
        _modifyUserService = modifyUserService;
        _deleteUserService = deleteUserService;
        _consultaUsuarioService = consultaUsuarioService;
    }

    public SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto)
    {
        return _altaUsuarioService.SignUpClient(requestDto);
    }

    public UsuarioResponseDTO CrearUsuarioInterno(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto)
    {
        return _altaUsuarioService.MakeInterUser(idUsuarioLogueado, requestDto);
    }

    public UsuarioResponseDTO ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto)
    {
        return _modifyUserService.UpdateInternalUser(
            idUsuarioAModificar,
            idUsuarioLogueado,
            requestDto
        );
    }

    public void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto)
    {
        _modifyUserService.UpdateUserPassword(
            idUsuarioAModificar,
            idUsuarioLogueado,
            requestDto
        );
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        _deleteUserService.DeleteInterUser(idUsuarioAEliminar, idUsuarioLogueado);
    }

    public List<UsuarioResponseDTO> BuscarUsuarios(int idUsuarioLogueado, FindUserRequestDTO requestDto)
    {
        return _consultaUsuarioService.BuscarUsuarios(idUsuarioLogueado, requestDto);
    }
}
