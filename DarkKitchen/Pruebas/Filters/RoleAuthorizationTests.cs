using System.Security.Claims;
using Contratos;
using DarkKitchen.Filters;
using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Pruebas.Filters;

[TestClass]
public class RoleAuthorizationTests
{
    private static AuthorizationFilterContext CreateContextWithUser(params Claim[] claims)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());
    }

    [TestMethod]
    public void OnAuthorization_WhenUserHasNoRoleClaim_SetsForbiddenResult()
    {
        var filter = new RoleAuthorizationAttribute(UserRoles.Administrative);
        var context = CreateContextWithUser();

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El usuario no tiene permisos para ejecutar esta acción", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserRoleIsNotAllowed_SetsForbiddenResult()
    {
        var filter = new RoleAuthorizationAttribute(UserRoles.Administrative);
        var context = CreateContextWithUser(
            new Claim(ClaimTypes.Role, UserRoles.Client.ToString()));

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El usuario no tiene permisos para ejecutar esta acción", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserRoleIsAllowed_DoesNothing()
    {
        var filter = new RoleAuthorizationAttribute(UserRoles.Administrative);
        var context = CreateContextWithUser(
            new Claim(ClaimTypes.Role, UserRoles.Administrative.ToString()));

        filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserRoleIsInAllowedRoles_DoesNothing()
    {
        var filter = new RoleAuthorizationAttribute(
            UserRoles.Administrative,
            UserRoles.Preparer);

        var context = CreateContextWithUser(
            new Claim(ClaimTypes.Role, UserRoles.Preparer.ToString()));

        filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserRoleIsNotInAllowedRoles_SetsForbiddenResult()
    {
        var filter = new RoleAuthorizationAttribute(
            UserRoles.Administrative,
            UserRoles.Preparer);

        var context = CreateContextWithUser(
            new Claim(ClaimTypes.Role, UserRoles.Client.ToString()));

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("El usuario no tiene permisos para ejecutar esta acción", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_WhenRoleClaimIsEmpty_SetsForbiddenResult()
    {
        var filter = new RoleAuthorizationAttribute(UserRoles.Administrative);
        var context = CreateContextWithUser(
            new Claim(ClaimTypes.Role, string.Empty));

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
    }
}
