using System.Security.Claims;
using Contratos;
using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Pruebas.Filters;

[TestClass]
public class AuthenticationFilterTests
{
    private static AuthenticationFilter CreateFilter(
        Mock<ISessionValidationService>? serviceMock = null)
    {
        serviceMock ??= new Mock<ISessionValidationService>(MockBehavior.Strict);
        return new AuthenticationFilter(serviceMock.Object);
    }

    private static AuthorizationFilterContext CreateContext(
        string? sessionHeader = null,
        bool allowAnonymous = false)
    {
        var httpContext = new DefaultHttpContext();

        if(sessionHeader != null)
        {
            httpContext.Request.Headers["X-Session-Id"] = sessionHeader;
        }

        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = allowAnonymous
                ? new List<object> { new AllowAnonymousAttribute() }
                : new List<object>()
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        return new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());
    }

    [TestMethod]
    public void OnAuthorization_WhenHeaderIsMissing_SetsUnauthorizedResult()
    {
        var filter = CreateFilter();
        var context = CreateContext();

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El header de sesión es obligatorio", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenHeaderIsEmpty_SetsUnauthorizedResult()
    {
        var filter = CreateFilter();
        var context = CreateContext(string.Empty);

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El header de sesión es obligatorio", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenSessionIdIsInvalidGuid_SetsUnauthorizedResult()
    {
        var filter = CreateFilter();
        var context = CreateContext("abc");

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El identificador de sesión es inválido", response.Message);
    }

   [TestMethod]
public void OnAuthorization_WhenSessionIdIsValid_CallsSessionValidationService()
{
    var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
    var sessionId = Guid.NewGuid();

    var currentUser = new CurrentUserDto
    {
        Id = 5,
        Role = "Client",
        Email = "miguel@gmail.com"
    };

    serviceMock
        .Setup(x => x.ValidateSession(sessionId))
        .Returns(currentUser);

    var filter = CreateFilter(serviceMock);
    var context = CreateContext(sessionId.ToString());

    filter.OnAuthorization(context);

    serviceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
}

[TestMethod]
public void OnAuthorization_WhenEndpointAllowsAnonymous_DoesNothing()
{
    var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
    var filter = CreateFilter(serviceMock);
    var context = CreateContext(allowAnonymous: true);

    filter.OnAuthorization(context);

    Assert.IsNull(context.Result);
}

[TestMethod]
public void OnAuthorization_WhenSessionIsValid_SetsAuthenticatedUser()
{
    var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
    var sessionId = Guid.NewGuid();

    var currentUser = new CurrentUserDto
    {
        Id = 5,
        Role = "Client",
        Email = "miguel@gmail.com"
    };

    serviceMock
        .Setup(x => x.ValidateSession(sessionId))
        .Returns(currentUser);

    var filter = new AuthenticationFilter(serviceMock.Object);
    var context = CreateContext(sessionId.ToString());

    filter.OnAuthorization(context);

    Assert.IsNotNull(context.HttpContext.User);
    Assert.IsTrue(context.HttpContext.User.Identity?.IsAuthenticated);

    serviceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
}

[TestMethod]
public void OnAuthorization_WhenSessionIsValid_SetsNameIdentifierClaim()
{
    var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
    var sessionId = Guid.NewGuid();

    var currentUser = new CurrentUserDto
    {
        Id = 5,
        Role = "Client",
        Email = "miguel@gmail.com"
    };

    serviceMock
        .Setup(x => x.ValidateSession(sessionId))
        .Returns(currentUser);

    var filter = CreateFilter(serviceMock);
    var context = CreateContext(sessionId.ToString());

    filter.OnAuthorization(context);

    var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

    Assert.IsNotNull(userIdClaim);
    Assert.AreEqual("5", userIdClaim!.Value);

    serviceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
}

[TestMethod]
public void OnAuthorization_WhenSessionIsValid_SetsRoleClaim()
{
    var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
    var sessionId = Guid.NewGuid();

    var currentUser = new CurrentUserDto
    {
        Id = 5,
        Role = "Client",
        Email = "miguel@gmail.com"
    };

    serviceMock
        .Setup(x => x.ValidateSession(sessionId))
        .Returns(currentUser);

    var filter = CreateFilter(serviceMock);
    var context = CreateContext(sessionId.ToString());

    filter.OnAuthorization(context);

    var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);

    Assert.IsNotNull(roleClaim);
    Assert.AreEqual("Client", roleClaim!.Value);

    serviceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
}

[TestMethod]
public void OnAuthorization_WhenSessionValidationThrows_SetsUnauthorizedResult()
{
    var sessionId = Guid.NewGuid();
    var sessionValidationServiceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);

    sessionValidationServiceMock
        .Setup(x => x.ValidateSession(sessionId))
        .Throws(new UserAuthenticationException("Sesión inválida o expirada"));

    var filter = new AuthenticationFilter(sessionValidationServiceMock.Object);
    var context = CreateContext(sessionId.ToString());

    filter.OnAuthorization(context);

    var result = context.Result as ObjectResult;
    Assert.IsNotNull(result);
    Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);

    var response = result.Value as ResponseDto;
    Assert.IsNotNull(response);
    Assert.IsFalse(response.ExecutionSuccessful);
    Assert.AreEqual("Sesión inválida o expirada", response.Message);

    sessionValidationServiceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
}
}
