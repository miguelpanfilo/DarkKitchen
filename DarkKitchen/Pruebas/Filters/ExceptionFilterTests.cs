using Contratos;
using DarkKitchen.Filters;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pruebas.Filters;

[TestClass]
public class ExceptionFilterTests
{
    [TestMethod]
    public void OnException_UserValidationException_ReturnsBadRequest()
    {
        var filter = new ExceptionFilter();

        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext
        {
            HttpContext = httpContext
        };

        var exceptionContext = new ExceptionContext(
            actionContext,
            new List<IFilterMetadata>())
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
}
