using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.AuditLogs;
using Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class AuditLogsControllerTests
{
    [TestMethod]
    public void GetByFilters_ValidRequest_Returns200WithLogs()
    {
        var auditServiceMock = new
            Mock<IAuditLogService>(MockBehavior.Strict);

        var request = new AuditLogFilterRequest
        {
            DateTimeFrom = new DateTime(2026, 1, 1, 8, 0, 0),
            DateTimeTo = new DateTime(2026, 1, 1, 10, 0, 0),
            EntityName = "PRODUCTO",
            EntityId = 5
        };

        var logs = new List<AuditLogResponse>
        {
            new AuditLogResponse { Id = 1, EntityName = "PRODUCTO",
                EntityId = 5 }
        };

        auditServiceMock
            .Setup(s => s.GetByFilters(request))
            .Returns(logs);

        var controller = new
            AuditLogsController(auditServiceMock.Object);

        var result = controller.GetByFilters(request) as
            OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        auditServiceMock.Verify(s => s.GetByFilters(request),
            Times.Once);
    }
}
