using BusinessLogics;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Users;
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
}
