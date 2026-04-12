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
    [TestMethod]
    public void OnAuthorization_WhenHeaderIsMissing_ThrowsException()
    {
        var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
        var filter = new AuthenticationFilter(serviceMock.Object);
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            filter.OnAuthorization(context));

        Assert.AreEqual("El header de sesión es obligatorio", ex.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenHeaderIsEmpty_ThrowsException()
    {
        var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
        var filter = new AuthenticationFilter(serviceMock.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Session-Id"] = string.Empty;

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        var ex = Assert.ThrowsException<UserAuthenticationException>(() =>
            filter.OnAuthorization(context));

        Assert.AreEqual("El header de sesión es obligatorio", ex.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenSessionIdIsInvalidGuid_ThrowsException()
    {
        var serviceMock = new Mock<ISessionValidationService>(MockBehavior.Strict);
        var filter = new AuthenticationFilter(serviceMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Session-Id"] = "abc";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

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

        var filter = new AuthenticationFilter(serviceMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Session-Id"] = sessionId.ToString();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        filter.OnAuthorization(context);

        serviceMock.Verify(x => x.ValidateSession(sessionId), Times.Once);
    }
}
