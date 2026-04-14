using AlmacenamientoAbstracciones;
using Contratos;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class GetClientService(IRepositoryUser repositoryUser) : IGetClientService
{
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public UsuarioResponseDTO GetCurrentClient(int loggedUserId)
    {
        _repositoryUser.FindById(loggedUserId);
        return new UsuarioResponseDTO();
    }
}
