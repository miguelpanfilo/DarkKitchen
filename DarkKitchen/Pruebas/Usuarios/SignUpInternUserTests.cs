using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Mapeo;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class SignUpInternUserTests
{
    [TestMethod]
    public void CreateInternalUser_ValidData_ReturnsUserResponse()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        var loggedInUser = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        loggedInUser.Id = 1;

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
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

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

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Phone is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_NameWithNumbers_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            User.MakeIntern(
                "Ju4n",
                "Perez",
                "admin@gmail.com",
                Phone.MakePhone("099123456"),
                "PasswordSeguraee3!",
                UserRoles.Administrative));

        Assert.AreEqual("Name cannot contain numbers", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidPhone_ThrowsExceptionAndDoesNotCallRepository()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

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

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Phone is not valid", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_CallsRepositoryOnce()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

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

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_EmailAlreadyRegistered_DoesNotCallAdd()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.EmailExist("admin@gmail.com"))
            .Returns(true);

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

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Email is already registered", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_InvalidRole_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1));

        phoneValidatorMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

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
            Role = "Anything"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("Internal role is not valid", ex.Message);
    }

    [TestMethod]
    public void MapDTOToInternalUser_ValidData_ReturnsInternalUser()
    {
        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrativo"
        };

        var phone = Phone.MakePhone("099123456");
        var role = UserRoles.Administrative;

        var user = request.MappingDTOToInternUser(phone, role);

        Assert.AreEqual("Juan", user.Name);
        Assert.AreEqual("Perez", user.LastName);
        Assert.AreEqual("admin@gmail.com", user.Email.ToString());
        Assert.AreEqual("099123456", user.Phone.Value);
        Assert.AreEqual(UserRoles.Administrative, user.Role);
    }

    [TestMethod]
    public void MapUserToResponseDTO_ValidUser_MapsCorrectly()
    {
        var user = User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative);

        user.Id = 10;

        var response = user.MappingUserToResponseDTO();

        Assert.AreEqual(10, response.Id);
        Assert.AreEqual("Juan", response.Name);
        Assert.AreEqual("Perez", response.LastName);
        Assert.AreEqual("admin@gmail.com", response.Email);
        Assert.AreEqual("Administrative", response.Role);
    }

    [TestMethod]
    public void MapUserToResponseDTO_NullUser_ThrowsControlledException()
    {
        User user = null!;

        var ex = Assert.ThrowsException<UserException>(user.MappingUserToResponseDTO);

        Assert.AreEqual("User cannot be null", ex.Message);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void CreateIntern_ClientRole_ThrowsException()
    {
        User.MakeIntern(
            "Juan",
            "Perez",
            "admin@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Cliente);
    }

    [TestMethod]
    public void CreateIntern_EmptyName_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            User.MakeIntern(
                string.Empty,
                "Perez",
                "admin@gmail.com",
                Phone.MakePhone("099123456"),
                "PasswordSeguraee3!",
                UserRoles.Administrative));

        Assert.AreEqual("Name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateIntern_EmptyLastName_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            User.MakeIntern(
                "Juan",
                string.Empty,
                "admin@gmail.com",
                Phone.MakePhone("099123456"),
                "PasswordSeguraee3!",
                UserRoles.Administrative));

        Assert.AreEqual("Last name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateIntern_LastNameLessThanThreeCharacters_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            User.MakeIntern(
                "Juan",
                "Pe",
                "admin@gmail.com",
                Phone.MakePhone("099123456"),
                "PasswordSeguraee3!",
                UserRoles.Administrative));

        Assert.AreEqual("Last name must be between 3 and 25 characters", ex.Message);
    }

    [TestMethod]
    public void CreateIntern_LastNameGreaterThanTwentyFiveCharacters_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            User.MakeIntern(
                "Juan",
                "ApellidoDemasiadoDemasiadoLargo",
                "admin@gmail.com",
                Phone.MakePhone("099123456"),
                "PasswordSeguraee3!",
                UserRoles.Administrative));

        Assert.AreEqual("Last name must be between 3 and 25 characters", ex.Message);
    }

    [TestMethod]
    public void CreatePhone_Empty_ThrowsException()
    {
        var ex = Assert.ThrowsException<UserException>(() =>
            Phone.MakePhone(string.Empty));

        Assert.AreEqual("Phone cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_CallsEmailExistOnce()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

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

        repoMock.Verify(r => r.EmailExist("admin@gmail.com"), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_ValidData_ValidatesPhoneOnce()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

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

        phoneValidatorMock.Verify(v => v.ItsValid("099123456"), Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_NonAdministrativeUser_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new UserException("Only an administrative user can perform this action"));

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO();

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(5, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_LoggedInUserDoesNotExist_ThrowsException()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new UserException("User does not exist"));

        var service = new SignUpService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var request = new SingUpInternUserRequestDTO();

        var ex = Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void CreateInternalUser_CallsAdminValidationService()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

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
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new UserException("Only an administrative user can perform this action"));

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

        Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_NotAdmin_DoesNotValidatePhone()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new UserException("Only an administrative user can perform this action"));

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

        Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        phoneValidatorMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void CreateInternalUser_NotAdmin_DoesNotCheckRepeatedEmail()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var phoneValidatorMock = new Mock<IPhoneValidator>();
        var adminValidationMock = new Mock<IAdminValidationService>();

        adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(1))
            .Throws(new UserException("Only an administrative user can perform this action"));

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

        Assert.ThrowsException<UserException>(() =>
            service.MakeInterUser(1, request));

        repoMock.Verify(r => r.EmailExist(It.IsAny<string>()), Times.Never);
    }
}
