using BusinessLogics;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Users;
using Contract;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.AuditLogs;

[TestClass]
public class AuditLogServiceTests
{
    [TestMethod]
    public void Register_ValidData_CallsRepositoryAdd()
    {
        var repoMock = new
            Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        currentUserMock
            .Setup(s => s.GetEmail())
            .Returns("admin@dark.com");

        repoMock
            .Setup(r => r.Add(It.IsAny<AuditLog>()));

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        service.Register(AuditedEntities.Product, 5,
            AuditDescriptions.ProductCreated);

        repoMock.Verify(r => r.Add(It.IsAny<AuditLog>()), Times.Once);
    }

    [TestMethod]
    public void GetByFilters_MissingDateFrom_ThrowsValidationException()
    {
        var repoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = null,
            DateTimeTo = DateTime.UtcNow.AddHours(1),
            EntityName = AuditedEntities.Product,
            EntityId = 5
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetByFilters(request));

        Assert.AreEqual("La fecha-hora desde es obligatoria.", ex.Message);
    }

    [TestMethod]
    public void GetByFilters_MissingDateTo_ThrowsValidationException()
    {
        var repoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = DateTime.UtcNow,
            DateTimeTo = null,
            EntityName = AuditedEntities.Product,
            EntityId = 5
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetByFilters(request));

        Assert.AreEqual("La fecha-hora hasta es obligatoria.", ex.Message);
    }

    [TestMethod]
    public void
        GetByFilters_DateFromGreaterThanDateTo_ThrowsValidationException()
    {
        var repoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = DateTime.UtcNow.AddHours(2),
            DateTimeTo = DateTime.UtcNow,
            EntityName = AuditedEntities.Product,
            EntityId = 5
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetByFilters(request));

        Assert.AreEqual("La fecha-hora desde debe ser menor que la fecha-hora hasta.", ex.Message);
    }

    [TestMethod]
    public void GetByFilters_MissingEntityName_ThrowsValidationException()
    {
        var repoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = DateTime.UtcNow,
            DateTimeTo = DateTime.UtcNow.AddHours(1),
            EntityName = string.Empty,
            EntityId = 5
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetByFilters(request));

        Assert.AreEqual("La entidad es obligatoria.", ex.Message);
    }
}
