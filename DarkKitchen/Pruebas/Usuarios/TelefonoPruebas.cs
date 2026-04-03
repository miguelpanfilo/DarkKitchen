using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class TelefonoPruebas
{
    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void CrearTelefono_Vacio_LanzaExcepcion()
    {
        var validadorMock = new Mock<IValidadorTelefono>();

        Telefono.Crear("", validadorMock.Object);
    }
}
