using DarkKitchen.Filters;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Pruebas.Filters;

[TestClass]
public class AuthenticationFilterTests
{
    [TestMethod]
    public void OnAuthorization_WhenHeaderIsMissing_ThrowsException()
    {
        var filter = new AuthenticationFilter();

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
        var filter = new AuthenticationFilter();

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
}
