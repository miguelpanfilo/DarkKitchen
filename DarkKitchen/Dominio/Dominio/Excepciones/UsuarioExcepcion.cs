namespace Dominio.Excepciones;

public class UsuarioExcepcion : Exception
{
    public UsuarioExcepcion(string mensaje)
        : base(mensaje)
    {
    }
}
