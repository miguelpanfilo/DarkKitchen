using Contratos;
using Dominio;

namespace Mapeo;

public static class UsuarioMapper
{
    public static Usuario MapeoDTOAUserInterno(this CrearUsuarioInternoRequest request)
    {
        Enum.TryParse<RolUsuario>(request.Rol, true, out var rol);

        return Usuario.CrearInterno(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave,
            rol
        );
    }
}
