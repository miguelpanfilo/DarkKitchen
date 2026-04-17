using Contratos;
using DarkKitchen.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Pruebas.Pedidos;

[TestClass]
public sealed class RoleFilterTests
{
    private static AuthorizationFilterContext MakeContext(CurrentUserDto? user)
    {
        var httpContext = new DefaultHttpContext();
        if(user is not null)
        {
            httpContext.Items["CurrentUser"] = user;
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }

    private static CurrentUserDto MakeUser(UserRolesDto role) => new()
    {
        Id = 1,
        Role = role,
        Email = "user@test.com"
    };

    #region OnAuthorization
    #region Success
    [TestMethod]
    public void OnAuthorization_WhenUserHasAllowedRole_ShouldNotSetResult()
    {
        var filter = new RoleFilter(UserRolesDto.Client);
        var context = MakeContext(MakeUser(UserRolesDto.Client));

        filter.OnAuthorization(context);

        context.Result.Should().BeNull();
    }

    [TestMethod]
    public void OnAuthorization_WhenUserHasOneOfMultipleAllowedRoles_ShouldNotSetResult()
    {
        var filter = new RoleFilter(UserRolesDto.Preparer, UserRolesDto.Administrative);
        var context = MakeContext(MakeUser(UserRolesDto.Administrative));

        filter.OnAuthorization(context);

        context.Result.Should().BeNull();
    }
    #endregion

    #region Error
    [TestMethod]
    public void OnAuthorization_WhenUserHasDisallowedRole_ShouldSetForbiddenResult()
    {
        var filter = new RoleFilter(UserRolesDto.Client);
        var context = MakeContext(MakeUser(UserRolesDto.Preparer));

        filter.OnAuthorization(context);

        context.Result.Should().NotBeNull();
        context.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [TestMethod]
    public void OnAuthorization_WhenUserIsNull_ShouldSetForbiddenResult()
    {
        var filter = new RoleFilter(UserRolesDto.Client);
        var context = MakeContext(null);

        filter.OnAuthorization(context);

        context.Result.Should().NotBeNull();
        context.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
    #endregion
    #endregion
}
