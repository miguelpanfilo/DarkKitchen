using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Mapeo;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class SignUpInternUserTests
{
    [TestMethod]
    public void CreateInternalUser_ValidData_ReturnsUserResponse()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        loggedInUser.Id = 1;

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);

        repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(false);

        repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        var result = service.MakeInterUser(1, request);

        Assert.AreEqual(10, result.Id);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidPhone_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock.Setup(v => v.ItsValid("123")).Returns(false);

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "123",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        var ex = Assert.ThrowsException<SignUpValidationException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Phone is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidPhone_ThrowsExceptionAndDoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        phoneValidatorMock.Setup(v => v.ItsValid("091123456")).Returns(false);

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "091123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        var ex = Assert.ThrowsException<SignUpValidationException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Phone is not valid", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_CallsRepositoryOnce()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);
        repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(false);

        repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        service.MakeInterUser(1, request);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_EmailAlreadyRegistered_DoesNotCallAdd()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);
        repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "AdmiNistrAtIve"
        };

        var ex = Assert.ThrowsException<SignUpException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Email is already registered", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidRole_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Role = "Anything"
        };

        var ex = Assert.ThrowsException<SignUpException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Internal role is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_NonAdministrativeUser_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(5, new SingUpInternUserRequestDTO()));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_CallsAdminValidationService()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.EmailExist("admin@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        service.MakeInterUser(1, request);

        adminValidationMock.Verify(v => v.EnsureUserIsAdmin(1), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_NotAdmin_DoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, new SingUpInternUserRequestDTO()));

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }
}
