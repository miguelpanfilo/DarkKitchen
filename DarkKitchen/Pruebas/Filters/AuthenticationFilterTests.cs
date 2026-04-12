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
    public void OnAuthorization_WhenHeaderIsMissing_ThrowsException()
    {
        var filter = CreateFilter();
        var context = CreateContext();

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            filter.OnAuthorization(context));

        Assert.AreEqual("El header de sesión es obligatorio", ex.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenHeaderIsEmpty_ThrowsException()
    {
        var filter = CreateFilter();
        var context = CreateContext(string.Empty);

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            filter.OnAuthorization(context));

        Assert.AreEqual("El header de sesión es obligatorio", ex.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenSessionIdIsInvalidGuid_ThrowsException()
    {
        var filter = CreateFilter();
        var context = CreateContext("abc");

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            filter.OnAuthorization(context));

        Assert.AreEqual("El identificador de sesión es inválido", ex.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenSessionIdIsValid_CallsSessionValidationService()
    {
        var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
        var sessionId = Guid.NewGuid();

        serviceMock
            .Setup(x => x.ValidateSession(sessionId));

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
}
