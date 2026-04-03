using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IAltaUsuarioService
{
    RegistrarClienteResponseDTO RegistrarCliente(RegistrarClienteRequest request);
    UsuarioResponse CrearUsuarioInterno(int idUsuarioLogueado, CrearUsuarioInternoRequest request);
}
