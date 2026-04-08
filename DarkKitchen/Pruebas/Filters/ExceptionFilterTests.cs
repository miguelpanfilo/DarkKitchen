using Contratos;
using DarkKitchen.Filters;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Pruebas.Filters;

[TestClass]
public class ExceptionFilterTests
{
    [TestMethod]
    public void OnException_UserValidationException_ReturnsBadRequest()
    {
        var filter = new ExceptionFilter();

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var filterMetadataMock = new Mock<IFilterMetadata>(MockBehavior.Strict);
        var filters = new List<IFilterMetadata> { filterMetadataMock.Object };

        var exceptionContext = new ExceptionContext(actionContext, filters)
        {
            Exception = new UserValidationException("Invalid data")
        };

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Invalid data", response.Message);
    }

    [TestMethod]
    public void OnException_UserAuthenticationException_ReturnsUnauthorized()
    {
        var filter = new ExceptionFilter();

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var filterMetadataMock = new Mock<IFilterMetadata>(MockBehavior.Strict);
        var filters = new List<IFilterMetadata> { filterMetadataMock.Object };

        var exceptionContext = new ExceptionContext(actionContext, filters)
        {
            Exception = new UserAuthenticationException("Invalid credentials")
        };

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Invalid credentials", response.Message);
    }

    [TestMethod]
    public void OnException_UserForbiddenException_ReturnsForbidden()
    {
        var filter = new ExceptionFilter();

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var filterMetadataMock = new Mock<IFilterMetadata>(MockBehavior.Strict);
        var filters = new List<IFilterMetadata> { filterMetadataMock.Object };

        var exceptionContext = new ExceptionContext(actionContext, filters)
        {
            Exception = new UserForbiddenException("Only an administrative user can perform this action")
        };

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Only an administrative user can perform this action", response.Message);
    }
}
