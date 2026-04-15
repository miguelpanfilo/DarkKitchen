using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class FilterUserTests
{
    private static Phone CreateValidPhone(string phone)
    {
        var phoneResult = Phone.TryMakePhone(phone);
        Assert.IsTrue(phoneResult.Success);
        Assert.IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    private static User CreateInternalUser(
        string name,
        string lastName,
        string email,
        string phone,
        string password,
        UserRoles role,
        int id)
    {
        var validPhone = CreateValidPhone(phone);

        var userResult = User.MakeIntern(
            name,
            lastName,
            email,
            validPhone,
            password,
            role);

        Assert.IsTrue(userResult.Success);
        Assert.IsNotNull(userResult.Value);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    private static User CreateClientUser(
        string name,
        string lastName,
        string email,
        string phone,
        string password,
        int id)
    {
        var validPhone = CreateValidPhone(phone);

        var userResult = User.MakeClient(
            name,
            lastName,
            email,
            validPhone,
            password);

        Assert.IsTrue(userResult.Success);
        Assert.IsNotNull(userResult.Value);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var usuario2 = CreateInternalUser(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var usuario2 = CreateInternalUser(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateClientUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            100);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var usuario2 = CreateInternalUser(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var usuario2 = CreateInternalUser(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Juan",
            "Gomez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var usuario2 = CreateInternalUser(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var usuario = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            service.BuscarUsuarios(5, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_NoLlamaGetAllYLanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            service.BuscarUsuarios(5, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);

        repoMock.Verify(r => r.GetAll(), Times.Never);
    }

    [TestMethod]
    public void SearchUsers_WithoutNameAndLastName_ReturnsAllUsers()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var user1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var user2 = CreateInternalUser(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

        repoMock
            .Setup(r => r.GetAll())
            .Returns([user1, user2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO();

        var result = service.BuscarUsuarios(99, request);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SearchUsers_ByNameAndLastName_ReturnsUsersMatchingBoth()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var user1 = CreateInternalUser(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            1);

        var user2 = CreateInternalUser(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            "099111222",
            "PasswordSeguraaa4!",
            UserRoles.Preparer,
            2);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

        repoMock
            .Setup(r => r.GetAll())
            .Returns([user1, user2]);

        var service = new FilterUserService(repoMock.Object, adminValidationMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        var result = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan", result[0].Name);
        Assert.AreEqual("Perez", result[0].LastName);
    }
}
