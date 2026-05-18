using BusinessLogics.Users;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Users;
using Contract.Users;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.Users;

[TestClass]
public class SignUpInternUserTests
{
    private Mock<IRepositoryUser> _repoMock = null!;
    private Mock<IPhoneValidator> _phoneValidatorMock = null!;
    private Mock<IAdminValidationService> _adminValidationMock = null!;
    private SignUpService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        _adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        _service = new SignUpService(
            _repoMock.Object,
            _phoneValidatorMock.Object,
            _adminValidationMock.Object);
    }

    private static SingUpInternUserRequestDTO CreateValidInternalUserRequest()
    {
        return new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_ReturnsUserResponse()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);
        _repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(false);

        _repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var request = CreateValidInternalUserRequest();

        var result = _service.MakeInterUser(1, request);

        Assert.AreEqual(10, result.Id);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidPhone_ThrowsException()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("123")).Returns(false);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "123",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.MakeInterUser(1, request));

        Assert.AreEqual("Telefono no valido", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidPhone_ThrowsExceptionAndDoesNotCallRepository()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("091123456")).Returns(false);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "091123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrative"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.MakeInterUser(1, request));

        Assert.AreEqual("Telefono no valido", ex.Message);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_CallsRepositoryOnce()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);
        _repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(false);

        _repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var request = CreateValidInternalUserRequest();

        _service.MakeInterUser(1, request);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_EmailAlreadyRegistered_DoesNotCallAdd()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);
        _repoMock.Setup(r => r.EmailExist("admin@gmail.com")).Returns(true);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "AdmiNistrAtIve"
        };

        var ex = Assert.ThrowsException<ConflictException>(() =>
            _service.MakeInterUser(1, request));

        Assert.AreEqual("Email ya registrado", ex.Message);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidRole_ThrowsException()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(1));
        _phoneValidatorMock.Setup(v => v.ItsValid("099123456")).Returns(true);

        var request = new SingUpInternUserRequestDTO
        {
            Role = "Anything"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.MakeInterUser(1, request));

        Assert.AreEqual("El rol no es válido", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_NonAdministrativeUser_ThrowsException()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            _service.MakeInterUser(5, new SingUpInternUserRequestDTO()));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_CallsAdminValidationService()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        _phoneValidatorMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        _repoMock
            .Setup(r => r.EmailExist("admin@gmail.com"))
            .Returns(false);

        _repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var request = CreateValidInternalUserRequest();

        _service.MakeInterUser(1, request);

        _adminValidationMock.Verify(v => v.EnsureUserIsAdmin(1), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_NotAdmin_DoesNotCallRepository()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        Assert.ThrowsException<ForbiddenException>(() =>
            _service.MakeInterUser(1, new SingUpInternUserRequestDTO()));

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }
}
