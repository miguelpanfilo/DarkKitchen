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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;
        var usuarioAEliminar = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioAEliminar.Id = 52;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        repoMock
            .Setup(r => r.FindById(52))
            .Returns(usuarioAEliminar);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        service.EliminarUsuarioInterno(52, 5);

        repoMock.Verify(r => r.Remove(52), Times.Once);
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
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(5, 5)
        );

        Assert.AreEqual("Un usuario no puede eliminarse a sí mismo", ex.Message);
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

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(5, 99)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);
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

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(5, 99)
        );

        Assert.AreEqual("El usuario no existe", ex.Message);

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_SiSeEliminaASiMismo_NoLlamaRemove_YLanzaExcepcion()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioExistente = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioExistente.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioExistente);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(5, 5)
        );

        Assert.AreEqual("Un usuario no puede eliminarse a sí mismo", ex.Message);

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_UsuarioLogueadoNoEsAdministrativo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioAEliminar = Usuario.CrearInterno(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Telefono.Crear("099111111"),
            "PasswordSeguraee3!",
            RolUsuario.Preparador
        );
        usuarioAEliminar.Id = 10;

        var usuarioLogueado = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(10))
            .Returns(usuarioAEliminar);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(10, 5)
        );

        Assert.AreEqual("Solo un usuario administrativo puede realizar esta acción", ex.Message);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_UsuarioLogueadoNoEsAdministrativo_NoLlamaRemove()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var validadorTelefonoMock = new Mock<IValidadorTelefono>();

        var usuarioAEliminar = Usuario.CrearInterno(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Telefono.Crear("099111111"),
            "PasswordSeguraee3!",
            RolUsuario.Preparador
        );
        usuarioAEliminar.Id = 10;

        var usuarioLogueado = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(10))
            .Returns(usuarioAEliminar);

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new UsuarioService(repoMock.Object, validadorTelefonoMock.Object);

        Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.EliminarUsuarioInterno(10, 5)
        );

        repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }
}
