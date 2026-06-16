using BusinessLogics;
using BusinessLogicsAbstractions;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.Users;

[TestClass]
public class DeleteUserTests
{
    private Mock<IRepositoryUser> _repoMock = null!;
    private Mock<IAdminValidationService> _adminValidationMock = null!;
    private DeleteUserService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);
        _service = new DeleteUserService(_repoMock.Object, _adminValidationMock.Object);
    }

    private static Phone CreateValidPhone()
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);
        return phoneResult.Value!;
    }

    private static User CreateAdminUser(int id = 52)
    {
        var phone = CreateValidPhone();

        var userResult = User.MakeIntern(
            "Juan",
            "Perez",
            "admin2@gmail.com",
            phone,
            "PasswordSeguraee3!",
            UserRoles.Administrative);

        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    [TestMethod]
    public void DeleteInternalUser_ValidData_CallsRemoveAndValidatesAllSteps()
    {
        var userToDelete = CreateAdminUser(52);

        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(5));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(52, 5, It.IsAny<string>()));
        _adminValidationMock.Setup(v => v.GetExistingUser(52)).Returns(userToDelete);
        _repoMock.Setup(r => r.Remove(52));

        _service.DeleteInterUser(52, 5);

        _repoMock.Verify(r => r.Remove(52), Times.Once);
        _adminValidationMock.Verify(v => v.EnsureUserIsAdmin(5), Times.Once);
        _adminValidationMock.Verify(v => v.EnsureIsNotSelfAction(52, 5, It.IsAny<string>()), Times.Once);
        _adminValidationMock.Verify(v => v.GetExistingUser(52), Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_WhenDeletingItself_DoesNotCallRemove()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(5));
        _adminValidationMock
            .Setup(v => v.EnsureIsNotSelfAction(5, 5, "Un usuario no puede eliminarse a sí mismo"))
            .Throws(new ConflictException("Un usuario no puede eliminarse a sí mismo"));

        var ex = Assert.ThrowsException<ConflictException>(() =>
            _service.DeleteInterUser(5, 5));

        Assert.AreEqual("Un usuario no puede eliminarse a sí mismo", ex.Message);
        _repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_UserDoesNotExist_DoesNotCallRemove()
    {
        _adminValidationMock.Setup(v => v.EnsureUserIsAdmin(99));
        _adminValidationMock.Setup(v => v.EnsureIsNotSelfAction(5, 99, It.IsAny<string>()));
        _adminValidationMock
            .Setup(v => v.GetExistingUser(5))
            .Throws(new NotFoundException("El usuario no existe"));

        var ex = Assert.ThrowsException<NotFoundException>(() =>
            _service.DeleteInterUser(5, 99));

        Assert.AreEqual("El usuario no existe", ex.Message);
        _repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void DeleteInternalUser_LoggedUserIsNotAdmin_DoesNotCallRemove()
    {
        _adminValidationMock
            .Setup(v => v.EnsureUserIsAdmin(5))
            .Throws(new ForbiddenException("Only an administrative user can perform this action"));

        var ex = Assert.ThrowsException<ForbiddenException>(() =>
            _service.DeleteInterUser(10, 5));

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
        _repoMock.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
    }
}
