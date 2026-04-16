using Contratos;

public interface IUserService
{
    SignUpClientResponseDTO RegistrarCliente(SignUpClientRequestDTO requestDto);

    UserResponseDTO CrearUsuarioInterno(int idUsuarioLogueado, SingUpInternUserRequestDTO requestDto);

    UserResponseDTO ModificarUsuarioInterno(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarUsuarioRequestDTO requestDto);

    void ModificarClaveUsuario(
        int idUsuarioAModificar,
        int idUsuarioLogueado,
        ModificarClaveRequestDTO requestDto);

    void EliminarUsuarioInterno(int idUsuarioAEliminar, int idUsuarioLogueado);

    List<UserResponseDTO> BuscarUsuarios(int idUsuarioLogueado, FindUserRequestDTO requestDto);
}
