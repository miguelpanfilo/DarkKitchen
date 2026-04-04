using Dominio;
using Dominio.Excepciones;

namespace Pruebas.Usuarios;

[TestClass]
public class PhoneTests
{
    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearTelefono_Vacio_LanzaExcepcion()
    {
        Phone.MakePhone(String.Empty);
    }
}
