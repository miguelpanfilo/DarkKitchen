using AlmacenamientoAbstracciones;
using Contratos;
using Contratos.Users;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocio.Users;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Users;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ModifyInternUserTest
{
    private Mock<IRepositoryUser> _repoMock = null!;
    private Mock<IPhoneValidator> _phoneValidatorMock = null!;
    private Mock<IAdminValidationService> _adminValidationMock = null!;
    private ModifyUserService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        _adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);
        _service = new ModifyUserService(
            _repoMock.Object,
            _phoneValidatorMock.Object,
            _adminValidationMock.Object);
    }

    private static Phone CreateValidPhone(string number)
    {
        var phoneResult = Phone.TryMakePhone(number);
        Assert.IsTrue(phoneResult.Success);
        Assert.IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    private static User CreateAdministrativeUser(
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

    [TestMethod]
    public void UpdateInternalUser_OnlyName_ReturnsUserWithUpdatedName()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { Name = "Carlos" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Carlos", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);

        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenUserModifiesItself_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new IConflictException("A user cannot modify itself"));

        var request = new ModifyUserRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<IConflictException>(() =>
            _service.UpdateInternalUser(5, 5, request));

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_NameAndLastName_ReturnsUpdatedUser()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { Name = "Carlos", LastName = "Gomez" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Carlos", result.Name);
        Assert.AreEqual("Gomez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);
        Assert.AreEqual("Administrative", result.Role);

        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyLastName_ReturnsUserWithUpdatedLastName()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { LastName = "Gomez" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Gomez", result.LastName);

        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_NoChangesProvided_KeepsExistingData()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO();

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("099123456", result.Phone);
        Assert.AreEqual("admin@gmail.com", result.Email);

        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyPhone_ReturnsUserWithUpdatedPhone()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _phoneValidatorMock
            .Setup(v => v.ItsValid("098111222"))
            .Returns(true);

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { Phone = "098111222" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("098111222", result.Phone);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("admin@gmail.com", result.Email);

        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidPhone_ThrowsExceptionWithCorrectMessage()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var request = new ModifyUserRequestDTO { Phone = "123" };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El teléfono no es válido", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneInvalidAccordingToValidator_DoesNotModifyPhone()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _phoneValidatorMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var request = new ModifyUserRequestDTO { Phone = "123" };

        Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("099123456", existingUser.Phone.Value);
    }

    [TestMethod]
    public void UpdateInternalUser_LastNameGreaterThanTwentyFiveCharacters_ThrowsExceptionWithCorrectMessage()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { LastName = "ApellidoDemasiadoDemasiadoLargo" };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El apellido tiene que tener entre 3 y 25 caracteres", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_Phone_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.GetExistingUser(It.IsAny<int>()))
            .Throws(new NotFoundException("User does not exist"));

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Phone = "098111222" };

        var ex = Assert.ThrowsException<NotFoundException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_PhoneDoesNotChange_DoesNotValidatePhone()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { Phone = "099123456" };

        _service.UpdateInternalUser(5, 99, request);

        _phoneValidatorMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        _repoMock.Verify(r => r.Update(existingUser), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_OnlyEmail_ReturnsUserWithUpdatedEmail()
    {
        var existingUser = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(existingUser);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.EmailExist("nuevo@gmail.com"))
            .Returns(false);

        _repoMock
            .Setup(r => r.Update(existingUser))
            .Returns(existingUser);

        var request = new ModifyUserRequestDTO { Email = "nuevo@gmail.com" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("nuevo@gmail.com", result.Email);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.LastName);
        Assert.AreEqual("099123456", result.Phone);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenLoggedUserIsNotAdmin_DoesNotCallGetExistingUser()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var request = new ModifyUserRequestDTO();

        Assert.ThrowsException<ForbiddenException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        _adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidEmailFormat_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock.Setup(r => r.EmailExist("correo-invalido")).Returns(false);

        var request = new ModifyUserRequestDTO { Email = "correo-invalido" };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Formato de Email inválido", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailAlreadyExists_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock.Setup(r => r.EmailExist("repetido@gmail.com")).Returns(true);

        var request = new ModifyUserRequestDTO { Email = "repetido@gmail.com" };

        var ex = Assert.ThrowsException<IConflictException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El email ya está registrado", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmailSame_DoesNotCallRepository()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(user))
            .Returns(user);

        var request = new ModifyUserRequestDTO { Email = "admin@gmail.com" };

        _service.UpdateInternalUser(5, 99, request);

        _repoMock.Verify(r => r.EmailExist(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_InvalidRole_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Preparer,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Role = "InvalidRole" };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El rol interno no es válido", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_RoleDifferentCase_UpdatesCorrectly()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Preparer,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(user))
            .Returns(user);

        var request = new ModifyUserRequestDTO { Role = "adMinisTrative" };

        var result = _service.UpdateInternalUser(5, 99, request);

        Assert.AreEqual("Administrative", result.Role);

        _repoMock.Verify(r => r.Update(user), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_CallsAdminValidationMethods()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            UserRoles.Administrative,
            5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(99));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()));

        _repoMock
            .Setup(r => r.Update(user))
            .Returns(user);

        _service.UpdateInternalUser(5, 99, new ModifyUserRequestDTO());

        _adminValidationMock.Verify(v => v.EnsureUserIsAdmin(99), Times.Once);
        _adminValidationMock.Verify(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()), Times.Once);
        _adminValidationMock.Verify(v => v.GetExistingUser(5), Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenLoggedUserIsNotAdmin_ThrowsException()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var request = new ModifyUserRequestDTO { Name = "Carlos" };

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenUserModifiesItself_DoesNotCallGetExistingUser()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new IConflictException("A user cannot modify itself"));

        var request = new ModifyUserRequestDTO { Name = "Carlos" };

        Assert.ThrowsException<IConflictException>(() =>
            _service.UpdateInternalUser(5, 5, request));

        _adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyName_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan", "Perez", "admin@gmail.com", "099123456",
            "PasswordSeguraee3!", UserRoles.Administrative, 5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Name = "   " };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El nombre no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyLastName_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan", "Perez", "admin@gmail.com", "099123456",
            "PasswordSeguraee3!", UserRoles.Administrative, 5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { LastName = "   " };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El apellido no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyEmail_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan", "Perez", "admin@gmail.com", "099123456",
            "PasswordSeguraee3!", UserRoles.Administrative, 5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Email = "   " };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El email no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyPhone_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan", "Perez", "admin@gmail.com", "099123456",
            "PasswordSeguraee3!", UserRoles.Administrative, 5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Phone = "   " };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El teléfono no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_EmptyRole_ThrowsException()
    {
        var user = CreateAdministrativeUser(
            "Juan", "Perez", "admin@gmail.com", "099123456",
            "PasswordSeguraee3!", UserRoles.Administrative, 5);

        _adminValidationMock.Setup(v => v.GetExistingUser(5)).Returns(user);
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        var request = new ModifyUserRequestDTO { Role = "   " };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateInternalUser(5, 99, request));

        Assert.AreEqual("El rol no puede estar vacío", ex.Message);
    }

    [TestMethod]
    public void UpdateInternalUser_WhenRequestIsValid_CallsRepositoryUpdateOnce()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!",
            UserRoles.Preparer);

        Assert.IsTrue(userResult.Success);
        var user = userResult.Value!;
        user.Id = 3;

        var request = new ModifyUserRequestDTO
        {
            Name = "Migue",
            LastName = "Martinez",
            Email = "lucia.wwww@gmail.com",
            Phone = "098123456",
            Role = "Preparer"
        };

        adminValidationMock
            .Setup(x => x.EnsureUserIsAdmin(1));

        adminValidationMock
            .Setup(x => x.EnsureIsNotSelfAction(
                3,
                1,
                "Un usuario no puede modificarse a sí mismo"));

        adminValidationMock
            .Setup(x => x.GetExistingUser(3))
            .Returns(user);

        phoneValidatorMock
            .Setup(x => x.ItsValid("098123456"))
            .Returns(true);

        repoMock
            .Setup(x => x.EmailExist("lucia.wwww@gmail.com"))
            .Returns(false);

        repoMock
            .Setup(x => x.Update(user))
            .Returns(user);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        var response = service.UpdateInternalUser(3, 1, request);

        Assert.IsNotNull(response);
        Assert.AreEqual("Migue", response.Name);
        Assert.AreEqual("Martinez", response.LastName);
        Assert.AreEqual("lucia.wwww@gmail.com", response.Email);
        Assert.AreEqual("098123456", response.Phone);

        repoMock.Verify(x => x.Update(user), Times.Once);
    }
}
