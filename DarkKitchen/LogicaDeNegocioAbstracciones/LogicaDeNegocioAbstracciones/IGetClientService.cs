using Contratos;

public interface IGetClientService
{
    UsuarioResponseDTO GetCurrentClient(int loggedUserId);
}
