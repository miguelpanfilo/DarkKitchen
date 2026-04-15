using Contratos;

public interface IUserService
{
    SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto);

    UsuarioResponseDTO CrearUsuarioInterno(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto);

    UsuarioResponseDTO ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto);

    void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto);

    void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado);

    List<UsuarioResponseDTO> BuscarUsuarios(int idUsuarioLogueado, FindUserRequestDTO requestDto);
}
