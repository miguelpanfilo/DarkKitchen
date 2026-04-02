using AlmacenamientoAbstracciones;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class EliminacionUsuarioPruebas
{
    [TestMethod]
    public void EliminarUsuarioInterno_DatosValidos_LlamaRemoveUnaVez()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        service.EliminarUsuarioInterno(5, 99);

        repoMock.Verify(r => r.Remove(5), Times.Once);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_SiSeEliminaASiMismo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        try
        {
            service.EliminarUsuarioInterno(5, 5);
            Assert.Fail("Se esperaba excepción");
        }
        catch (UsuarioExcepcion ex)
        {
            Assert.AreEqual("Un usuario no puede eliminarse a sí mismo", ex.Message);
        }
    }

    [TestMethod]
    public void EliminarUsuarioInterno_UsuarioNoExiste_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        try
        {
            service.EliminarUsuarioInterno(5, 99);
            Assert.Fail("Se esperaba excepción");
        }
        catch (UsuarioExcepcion ex)
        {
            Assert.AreEqual("El usuario no existe", ex.Message);
        }
    }

    [TestMethod]
    public void EliminarUsuarioInterno_UsuarioNoExiste_NoLlamaRemove()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        repoMock
            .Setup(r => r.FindById(5))
            .Returns((Usuario?)null);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        try
        {
            service.EliminarUsuarioInterno(5, 99);
        }
        catch (UsuarioExcepcion)
        {
        }

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_SiSeEliminaASiMismo_NoLlamaRemove()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        try
        {
            service.EliminarUsuarioInterno(5, 5);
        }
        catch (UsuarioExcepcion)
        {
        }

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }
}
