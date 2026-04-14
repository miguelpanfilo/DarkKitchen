using AlmacenamientoAbstracciones;
using Contratos;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class GetClientService(IRepositoryUser repositoryUser) : IGetClientService
{
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public UsuarioResponseDTO GetCurrentClient(int loggedUserId)
    {
        var user = _repositoryUser.FindById(loggedUserId);

        if(user == null)
        {
            throw new NotFoundException("Not found");
        }

        return new UsuarioResponseDTO();
    }
}
