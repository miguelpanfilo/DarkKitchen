using Contratos;
using Dominio;
using Dominio.Excepciones;

namespace Mapeo;

public static class UsuarioMapper
{
    public static Usuario MapeoDTOAUserInterno(this CrearUsuarioInternoRequest request)
    {
        return Usuario.CrearInterno(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave,
            ParsearRol(request.Rol)
        );
    }

    private static RolUsuario ParsearRol(string rol)
    {
        if (!Enum.TryParse<RolUsuario>(rol, true, out var rolEnum))
        {
            throw new UsuarioExcepcion("El rol interno no es válido");
        }

        return rolEnum;
    }
}
