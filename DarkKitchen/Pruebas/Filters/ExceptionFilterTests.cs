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
    public void OnException_UserValidationException_ReturnsBadRequest()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new UserValidationException("Invalid data"));

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
        var exceptionContext = CreateExceptionContext(
            new UserAuthenticationException("Invalid credentials"));

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
        var exceptionContext = CreateExceptionContext(
            new UserForbiddenException("Only an administrative user can perform this action"));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Only an administrative user can perform this action", response.Message);
    }

    [TestMethod]
    public void OnException_UserNotFoundException_ReturnsNotFound()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new UserNotFoundException("User does not exist"));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("User does not exist", response.Message);
    }

    [TestMethod]
    public void OnException_UserConflictException_ReturnsConflict()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new UserConflictException("Email is already registered"));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Email is already registered", response.Message);
    }

    [TestMethod]
    public void OnException_GenericException_ReturnsInternalServerError()
    {
        var filter = new ExceptionFilter();
        var exceptionContext = CreateExceptionContext(
            new Exception("Unexpected error"));

        filter.OnException(exceptionContext);

        var result = exceptionContext.Result as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);

        var response = result.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Internal server error", response.Message);
    }
}
