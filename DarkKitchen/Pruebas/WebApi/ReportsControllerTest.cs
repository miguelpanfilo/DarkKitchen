using Contratos.Reports;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Reports;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Pruebas.WebApi;

[TestClass]
public class ReportsControllerTest
{
    [TestMethod]
    public void GivenValidDates_WhenGettingTopProducts_ThenReturnsOk()
    {
        var reportServiceMock = new Mock<IReportService>(MockBehavior.Strict);

        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);
        var expectedResult = new List<TopProductReportItemDto>();

        reportServiceMock
            .Setup(s => s.GetTopProducts(dateFrom, dateTo))
            .Returns(expectedResult);

        var controller = new ReportsController(reportServiceMock.Object);

        var result = controller.GetTopProducts(dateFrom, dateTo);

        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreSame(expectedResult, okResult.Value);

        reportServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GivenReportsController_WhenGettingTopProducts_ThenHasHttpGetAttribute()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GetTopProducts));

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .FirstOrDefault() as HttpGetAttribute;

        Assert.IsNotNull(attribute);
        Assert.AreEqual("top-products", attribute.Template);
    }

    [TestMethod]
    public void GivenReportsController_WhenGettingTopProducts_ThenHasAdministrativeRoleAuthorization()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GetTopProducts));

        var attribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .FirstOrDefault() as RoleAuthorizationAttribute;

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GivenValidDates_WhenGettingSalesReport_ThenReturnsOk()
    {
        var reportServiceMock = new Mock<IReportService>(MockBehavior.Strict);

        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);
        var expectedResult = new SalesReportDto();

        reportServiceMock
            .Setup(s => s.GetSalesReport(dateFrom, dateTo))
            .Returns(expectedResult);

        var controller = new ReportsController(reportServiceMock.Object);

        var result = controller.GetSalesReport(dateFrom, dateTo);

        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreSame(expectedResult, okResult.Value);

        reportServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GivenReportsController_WhenGettingSalesReport_ThenHasHttpGetAttribute()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GetSalesReport));

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .FirstOrDefault() as HttpGetAttribute;

        Assert.IsNotNull(attribute);
        Assert.AreEqual("sales", attribute.Template);
    }

    [TestMethod]
    public void GivenReportsController_WhenGettingSalesReport_ThenHasAdministrativeRoleAuthorization()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GetSalesReport));

        var attribute = method!
            .GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .FirstOrDefault() as RoleAuthorizationAttribute;

        Assert.IsNotNull(attribute);
    }
}
