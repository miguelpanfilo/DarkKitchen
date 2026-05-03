using BusinessLogics.Users;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Users;
using Contract.Users;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.Usuarios;

[TestClass]
public class ModifyPasswordTests
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
        int id)
    {
        var validPhone = CreateValidPhone(phone);

        var userResult = User.MakeIntern(
            name,
            lastName,
            email,
            validPhone,
            password,
            UserRoles.Administrative);

        Assert.IsTrue(userResult.Success);
        Assert.IsNotNull(userResult.Value);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    [TestMethod]
    public void UpdateUserPassword_LoggedInUserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new NotFoundException("User does not exist"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<NotFoundException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_DatosValidos_CambiaLaClave()
    {
        var usuario = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            5);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        _repoMock
            .Setup(r => r.Update(usuario))
            .Returns(usuario);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        _service.UpdateUserPassword(5, 99, request);

        Assert.IsTrue(usuario.VerifyPassword("ClaveNuevaSegura4!"));
        _repoMock.Verify(r => r.Update(usuario), Times.Once);
    }

    [TestMethod]
    public void UpdateUserPassword_LoggedInUserIsNotAdministrative_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_WrongCurrentPassword_ThrowsExceptionWithCorrectMessage()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            5);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(user);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "Incorrecta123!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("La contraseña actual es incorrecta", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_InvalidNewPassword_ThrowsExceptionWithCorrectMessage()
    {
        var user = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            5);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(user);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "123"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("Formato de clave inválida", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_UserDoesNotExist_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new NotFoundException("User does not exist"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<NotFoundException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.AreEqual("User does not exist", ex.Message);
    }

    [TestMethod]
    public void ModificarClaveUsuario_ClaveActualIncorrecta_NoCambiaLaClave()
    {
        var usuario = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            5);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "Incorrecta123!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<ValidationException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        Assert.IsTrue(usuario.Password.VerifyHash("PasswordSeguraee3!"));
    }

    [TestMethod]
    public void UpdateUserPassword_WhenUserModifiesItself_ThrowsExceptionWithCorrectMessage()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new ConflictException("A user cannot modify itself"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        var ex = Assert.ThrowsException<ConflictException>(() =>
            _service.UpdateUserPassword(5, 5, request));

        Assert.AreEqual("A user cannot modify itself", ex.Message);
    }

    [TestMethod]
    public void UpdateUserPassword_WhenLoggedUserIsNotAdmin_DoesNotCallGetExistingUser()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<ForbiddenException>(() =>
            _service.UpdateUserPassword(5, 99, request));

        _adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateUserPassword_WhenUserModifiesItself_DoesNotCallGetExistingUser()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(It.IsAny<int>()));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, It.IsAny<string>()))
            .Throws(new ConflictException("A user cannot modify itself"));

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        Assert.ThrowsException<ConflictException>(() =>
            _service.UpdateUserPassword(5, 5, request));

        _adminValidationMock.Verify(v => v.GetExistingUser(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void UpdateUserPassword_CallsAdminValidationMethods()
    {
        var usuario = CreateAdministrativeUser(
            "Juan",
            "Perez",
            "admin@gmail.com",
            "099123456",
            "PasswordSeguraee3!",
            5);

        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(99));

        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()));

        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Returns(usuario);

        _repoMock
            .Setup(r => r.Update(usuario))
            .Returns(usuario);

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        _service.UpdateUserPassword(5, 99, request);

        _adminValidationMock.Verify(v => v.EnsureUserIsAdmin(99), Times.Once);
        _adminValidationMock.Verify(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()), Times.Once);
        _adminValidationMock.Verify(v => v.GetExistingUser(5), Times.Once);
        _repoMock.Verify(r => r.Update(usuario), Times.Once);
    }

    [TestMethod]
    public void UpdateUserPassword_WhenRequestIsValid_CallsRepositoryUpdateOnce()
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

        var request = new ModifyPasswordRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
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

        repoMock
            .Setup(x => x.Update(user))
            .Returns(user);

        var service = new ModifyUserService(
            repoMock.Object,
            phoneValidatorMock.Object,
            adminValidationMock.Object);

        service.UpdateUserPassword(3, 1, request);

        Assert.IsTrue(user.VerifyPassword("ClaveNuevaSegura4!"));
        repoMock.Verify(x => x.Update(user), Times.Once);
    }
}
