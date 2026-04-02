using Contratos;
using Dominio;
using Dominio.Excepciones;

namespace Mapeo;

public static class UsuarioMapper
{
    public static Usuario MapeoDTOAUserInterno(this CrearUsuarioInternoRequest request)
    {
        var rolEnum = ParsearRol(request.Rol);

        return Usuario.CrearInterno(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            request.Clave,
            rolEnum
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

    public static UsuarioResponse MapeoAResponseDTO(this Usuario usuario)
    {
        if (usuario == null)
        {
            throw new UsuarioExcepcion("El usuario no puede ser null");
        }

        return new UsuarioResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email.ToString(),
            Rol = usuario.Rol.ToString(),
            Telefono = usuario.Telefono
        };
    }
}
