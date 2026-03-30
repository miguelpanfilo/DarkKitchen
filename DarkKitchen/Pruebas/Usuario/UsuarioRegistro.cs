using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UsuarioServiceTests
{
    [TestMethod]
    public void RegistrarCliente_DatosValidos_RetornaUsuario()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) => u);

        var service = new UsuarioService(repoMock.Object);

        var request = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Password = "Password@2026Ok"
        };

        var resultado = service.RegistrarCliente(request);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Miguel", resultado.Nombre);
        Assert.AreEqual("Lopez", resultado.Apellido);
        Assert.AreEqual("miguel@gmail.com", resultado.Email);
    }
}
