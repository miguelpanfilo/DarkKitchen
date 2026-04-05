using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class FilterUserTests
{
    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer);
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario1, usuario2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Juan", resultado[1].Name);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_FiltraSoloLosUsuariosConEseNombre()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer);
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario1, usuario2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreConDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "juAn"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns([]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_MapeaCorrectamenteLosDatosDelUsuario()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeClient(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!");
        usuario.Id = 100;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].Id);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Client", resultado[0].Role);
        Assert.AreEqual("099123456", resultado[0].Phone);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer);
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario1, usuario2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("Perez", resultado[1].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_NoImportaMaysculasMinusculas_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer);
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario1, usuario2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "PeReZ"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("Perez", resultado[1].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Juan",
            "Gomez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_FiltraSoloLosUsuariosConEseApellido()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer);
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario1, usuario2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("099123456", resultado[0].Phone);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Administrative", resultado[0].Role);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns([]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ConEspacios()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "  Perez   "
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_ConEspacios()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "   Juan  "
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
    }

    [TestMethod]
    public void BuscarUsuarios_RequestConEspaciosYDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns([usuario]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = " juan ",
            LastName = " pErEz "
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.BuscarUsuarios(5, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_NoLlamaGetAllYLanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.BuscarUsuarios(5, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);

        repoMock.Verify(r => r.GetAll(), Times.Never);
    }
}
