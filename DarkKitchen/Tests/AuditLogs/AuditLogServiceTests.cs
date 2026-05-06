using BusinessLogics;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Users;
using Contract;
using DataAccess.Repositories;
using DataAccessAbstractions;
using Domain;
using Moq;
using Tests.Persistence;

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

    [TestMethod]
    public void GetByFilters_MissingEntityId_ThrowsValidationException()
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
            EntityName = AuditedEntities.Product,
            EntityId = null
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.GetByFilters(request));

        Assert.AreEqual("El id de la entidad es obligatorio.",
            ex.Message);
    }

    [TestMethod]
    public void GetByFilters_ValidRequest_ReturnsLogsFromRepository()
    {
        var repoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        var from = new DateTime(2026, 1, 1, 8, 0, 0);
        var to = new DateTime(2026, 1, 1, 10, 0, 0);

        var logsFromRepo = new List<AuditLog>
        {
            new AuditLog
            {
                Id = 1,
                EntityName = AuditedEntities.Product,
                EntityId = 5,
                ChangeDescription = AuditDescriptions.ProductCreated,
                ResponsibleUser = "admin@dark.com",
                DateTime = from.AddMinutes(30)
            }
        };

        repoMock
            .Setup(r => r.GetByFilters(from, to, AuditedEntities.Product,
                5))
            .Returns(logsFromRepo);

        var service = new AuditLogService(repoMock.Object,
            currentUserMock.Object);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = from,
            DateTimeTo = to,
            EntityName = AuditedEntities.Product,
            EntityId = 5
        };

        var result = service.GetByFilters(request);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].Id);
        Assert.AreEqual(AuditedEntities.Product, result[0].EntityName);
        Assert.AreEqual(5, result[0].EntityId);
        Assert.AreEqual(AuditDescriptions.ProductCreated,
            result[0].ChangeDescription);
        Assert.AreEqual("admin@dark.com", result[0].ResponsibleUser);
    }

    [TestMethod]
    public void Add_ValidLog_IsSavedToDatabase()
    {
        var context = SqliteTestContextFactory.CreateContext(out var
            connection);
        var repo = new AuditLogRepository(context);

        var log = new AuditLog
        {
            EntityName = AuditedEntities.Product,
            EntityId = 5,
            ChangeDescription = AuditDescriptions.ProductCreated,
            ResponsibleUser = "admin@dark.com"
        };

        repo.Add(log);

        var saved = context.AuditLogs.FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(AuditedEntities.Product, saved.EntityName);
        Assert.AreEqual(5, saved.EntityId);

        context.Dispose();
        connection.Dispose();
    }

    [TestMethod]
    public void GetByFilters_MatchingLog_ReturnsIt()
    {
        var context = SqliteTestContextFactory.CreateContext(out var
            connection);
        var repo = new AuditLogRepository(context);

        var from = new DateTime(2026, 1, 1, 8, 0, 0);
        var to = new DateTime(2026, 1, 1, 10, 0, 0);

        var log = new AuditLog
        {
            EntityName = AuditedEntities.Product,
            EntityId = 5,
            ChangeDescription = AuditDescriptions.ProductCreated,
            ResponsibleUser = "admin@dark.com",
            DateTime = from.AddMinutes(30)
        };

        repo.Add(log);

        var result = repo.GetByFilters(from, to, AuditedEntities.Product,
            5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(AuditedEntities.Product, result[0].EntityName);
        Assert.AreEqual(5, result[0].EntityId);

        context.Dispose();
        connection.Dispose();
    }

    [TestMethod]
    public void GetByFilters_LogOutsideDateRange_IsNotReturned()
    {
        var context = SqliteTestContextFactory.CreateContext(out var
            connection);
        var repo = new AuditLogRepository(context);

        var from = new DateTime(2026, 1, 1, 8, 0, 0);
        var to = new DateTime(2026, 1, 1, 10, 0, 0);

        var log = new AuditLog
        {
            EntityName = AuditedEntities.Product,
            EntityId = 5,
            ChangeDescription = AuditDescriptions.ProductCreated,
            ResponsibleUser = "admin@dark.com",
            DateTime = new DateTime(2026, 1, 1, 11, 0, 0)
        };

        repo.Add(log);

        var result = repo.GetByFilters(from, to, AuditedEntities.Product,
            5);

        Assert.AreEqual(0, result.Count);

        context.Dispose();
        connection.Dispose();
    }

    [TestMethod]
    public void GetByFilters_LogFromDifferentEntity_IsNotReturned()
    {
        var context = SqliteTestContextFactory.CreateContext(out var
            connection);
        var repo = new AuditLogRepository(context);

        var from = new DateTime(2026, 1, 1, 8, 0, 0);
        var to = new DateTime(2026, 1, 1, 10, 0, 0);

        var log = new AuditLog
        {
            EntityName = AuditedEntities.Promotion,
            EntityId = 5,
            ChangeDescription = AuditDescriptions.PromotionCreated,
            ResponsibleUser = "admin@dark.com",
            DateTime = from.AddMinutes(30)
        };

        repo.Add(log);

        var result = repo.GetByFilters(from, to, AuditedEntities.Product,
            5);

        Assert.AreEqual(0, result.Count);

        context.Dispose();
        connection.Dispose();
    }

    [TestMethod]
    public void GetByFilters_LogFromDifferentEntityId_IsNotReturned()
    {
        var context = SqliteTestContextFactory.CreateContext(out var
            connection);
        var repo = new AuditLogRepository(context);

        var from = new DateTime(2026, 1, 1, 8, 0, 0);
        var to = new DateTime(2026, 1, 1, 10, 0, 0);

        var log = new AuditLog
        {
            EntityName = AuditedEntities.Product,
            EntityId = 99,
            ChangeDescription = AuditDescriptions.ProductCreated,
            ResponsibleUser = "admin@dark.com",
            DateTime = from.AddMinutes(30)
        };

        repo.Add(log);

        var result = repo.GetByFilters(from, to, AuditedEntities.Product,
            5);

        Assert.AreEqual(0, result.Count);

        context.Dispose();
        connection.Dispose();
    }
}
