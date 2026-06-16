using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Tests.Filters;

[TestClass]
public class ExceptionFilterTests
{
    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        var filterMetadataMock = new Mock<IFilterMetadata>(MockBehavior.Strict);
        var filters = new List<IFilterMetadata> { filterMetadataMock.Object };

        return new ExceptionContext(actionContext, filters)
        {
            Exception = exception
        };
    }

    [TestMethod]
    public void OnException_ValidationException_ReturnsBadRequest()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new ValidationException("Los datos ingresados no son válidos."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Los datos ingresados no son válidos.", response.Message);
    }

    [TestMethod]
    public void OnException_UserAuthenticationException_ReturnsUnauthorized()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new UserAuthenticationException("No fue posible autenticar al usuario."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No fue posible autenticar al usuario.", response.Message);
    }

    [TestMethod]
    public void OnException_ForbiddenException_ReturnsForbidden()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new ForbiddenException("No tienes permisos para realizar esta acción."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No tienes permisos para realizar esta acción.", response.Message);
    }

    [TestMethod]
    public void OnException_NotFoundException_ReturnsNotFound()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new NotFoundException("No se encontró el recurso solicitado."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No se encontró el recurso solicitado.", response.Message);
    }

    [TestMethod]
    public void OnException_ConflictException_ReturnsConflict()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new ConflictException("No fue posible completar la operación por un conflicto de datos."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No fue posible completar la operación por un conflicto de datos.", response.Message);
    }

    [TestMethod]
    public void OnException_GenericException_ReturnsInternalServerError()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new Exception("Ocurrió un error interno."));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Ocurrió un error interno.", response.Message);
    }
}
