using Contratos;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class UserService(
    ISignUpUserService signUpUserService,
    IModifyUserService modifyUserService,
    IDeleteUserService deleteUserService,
    IFilterUserService filterUserService)
{
    private readonly ISignUpUserService _signUpUserService = signUpUserService;
    private readonly IModifyUserService _modifyUserService = modifyUserService;
    private readonly IDeleteUserService _deleteUserService = deleteUserService;
    private readonly IFilterUserService _filterUserService = filterUserService;

    public SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto)
    {
        return _signUpUserService.SignUpClient(requestDto);
    }

    public UsuarioResponseDTO CrearUsuarioInterno(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto)
    {
        return _signUpUserService.MakeInterUser(idUsuarioLogueado, requestDto);
    }

    public UsuarioResponseDTO ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto)
    {
        return _modifyUserService.UpdateInternalUser(
            idUsuarioAModificar,
            idUsuarioLogueado,
            requestDto);
    }

    public void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto)
    {
        _modifyUserService.UpdateUserPassword(
            idUsuarioAModificar,
            idUsuarioLogueado,
            requestDto);
    }

    public void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado)
    {
        _deleteUserService.DeleteInterUser(idUsuarioAEliminar, idUsuarioLogueado);
    }

    public List<UsuarioResponseDTO> BuscarUsuarios(int idUsuarioLogueado, FindUserRequestDTO requestDto)
    {
        return _filterUserService.BuscarUsuarios(idUsuarioLogueado, requestDto);
    }
}
