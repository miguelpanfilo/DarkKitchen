using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract.Users;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.Logins;

[TestClass]
public class SessionsValidationServiceTests
{
    [TestMethod]
    public void ValidateSession_WhenSessionDoesNotExist_ThrowsException()
    {
        var sessionId = Guid.NewGuid();

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns((Session?)null);

        var service = new SessionsValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión no existe", ex.Message);

        sessionRepositoryMock.Verify(x => x.FindBySessionId(sessionId), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsExpired_ThrowsException()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddMinutes(-1));

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns(session);

        watchMock
            .Setup(x => x.Now())
            .Returns(now);

        var service = new SessionsValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión expiró", ex.Message);

        sessionRepositoryMock.Verify(x => x.FindBySessionId(sessionId), Times.Once);
        watchMock.Verify(x => x.Now(), Times.Once);
    }

   [TestMethod]
    public void ValidateSession_WhenSessionUserDoesNotExist_ThrowsException()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var sessionRepositoryMock = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepositoryMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watchMock = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepositoryMock
            .Setup(x => x.FindBySessionId(sessionId))
            .Returns(session);

        watchMock
            .Setup(x => x.Now())
            .Returns(now);

        userRepositoryMock
            .Setup(x => x.FindById(5))
            .Returns((User?)null);

        var service = new SessionsValidationService(
            sessionRepositoryMock.Object,
            userRepositoryMock.Object,
            watchMock.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("El usuario de la sesión no existe", ex.Message);

        userRepositoryMock.Verify(x => x.FindById(5), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsValid_ReturnsCurrentUserDto()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");
        Assert.IsTrue(userResult.Success);
        var user = userResult.Value!;
        user.Id = 5;

        var sessionRepo = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepo = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watch = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepo.Setup(r => r.FindBySessionId(sessionId)).Returns(session);
        watch.Setup(w => w.Now()).Returns(now);
        userRepo.Setup(r => r.FindById(5)).Returns(user);

        var service = new SessionsValidationService(
            sessionRepo.Object,
            userRepo.Object,
            watch.Object);

        var result = service.ValidateSession(sessionId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CurrentUserDto));

        var dto = (CurrentUserDto)result;
        Assert.AreEqual(5, dto.Id);
        Assert.AreEqual("Client", dto.Role.ToString());
        Assert.AreEqual("miguel@gmail.com", dto.Email);

        sessionRepo.Verify(r => r.FindBySessionId(sessionId), Times.Once);
        watch.Verify(w => w.Now(), Times.Once);
        userRepo.Verify(r => r.FindById(5), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenExpireDateEqualsNow_ThrowsException()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now);
        var sessionRepo = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepo = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watch = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepo.Setup(r => r.FindBySessionId(sessionId)).Returns(session);
        watch.Setup(w => w.Now()).Returns(now);

        var service = new SessionsValidationService(
            sessionRepo.Object,
            userRepo.Object,
            watch.Object);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            service.ValidateSession(sessionId));

        Assert.AreEqual("La sesión expiró", ex.Message);

        sessionRepo.Verify(r => r.FindBySessionId(sessionId), Times.Once);
        watch.Verify(w => w.Now(), Times.Once);
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsValid_WithPreparerRole_ReturnsCorrectRole()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!",
            UserRoles.Preparer);
        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 5;

        var sessionRepo = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepo = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watch = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepo.Setup(r => r.FindBySessionId(sessionId)).Returns(session);
        watch.Setup(w => w.Now()).Returns(now);
        userRepo.Setup(r => r.FindById(5)).Returns(user);

        var service = new SessionsValidationService(
            sessionRepo.Object,
            userRepo.Object,
            watch.Object);

        var result = service.ValidateSession(sessionId);

        Assert.IsNotNull(result);

        var dto = (CurrentUserDto)result;
        Assert.AreEqual("Preparer", dto.Role.ToString());
    }

    [TestMethod]
    public void ValidateSession_WhenSessionIsValid_WithAdministrativeRole_ReturnsCorrectRole()
    {
        var sessionId = Guid.NewGuid();
        var now = new DateTime(2026, 4, 12, 10, 0, 0);
        var session = new Session(5, now.AddHours(1));

        var phoneResult = Phone.TryMakePhone("099123456");
        Assert.IsTrue(phoneResult.Success);

        var userResult = User.MakeIntern(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phoneResult.Value!,
            "PasswordSeguraee3!",
            UserRoles.Administrative);
        Assert.IsTrue(userResult.Success);

        var user = userResult.Value!;
        user.Id = 5;

        var sessionRepo = new Mock<IRepositorySession>(MockBehavior.Strict);
        var userRepo = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var watch = new Mock<IWatch>(MockBehavior.Strict);

        sessionRepo.Setup(r => r.FindBySessionId(sessionId)).Returns(session);
        watch.Setup(w => w.Now()).Returns(now);
        userRepo.Setup(r => r.FindById(5)).Returns(user);

        var service = new SessionsValidationService(
            sessionRepo.Object,
            userRepo.Object,
            watch.Object);

        var result = service.ValidateSession(sessionId);

        Assert.IsNotNull(result);

        var dto = (CurrentUserDto)result;
        Assert.AreEqual("Administrative", dto.Role.ToString());
    }
}
