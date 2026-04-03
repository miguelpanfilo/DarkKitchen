using Dominio;
using Dominio.Excepciones;

namespace Pruebas.Usuarios;

[TestClass]
public class TelefonoPruebas
{
    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearTelefono_Vacio_LanzaExcepcion()
    {
        Telefono.Crear(String.Empty);
    }
}
