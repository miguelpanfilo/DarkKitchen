using DarkKitchen.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Pruebas.Filters;

[TestClass]
public class AuthenticationFilterTests
{
    [TestMethod]
    public void OnAuthorization_WhenEndpointAllowsAnonymous_DoesNothing()
    {
        var filter = new AuthenticationFilter();

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        actionContext.ActionDescriptor.EndpointMetadata.Add(new AllowAnonymousAttribute());

        var context = new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());

        filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }
}
