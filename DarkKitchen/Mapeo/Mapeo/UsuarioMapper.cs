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

    public static UsuarioResponse MapeoAResponseDTO(this Usuario? usuario)
    {
        return new UsuarioResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email.ToString(),
            Rol = usuario.Rol.ToString()
        };
    }
}
