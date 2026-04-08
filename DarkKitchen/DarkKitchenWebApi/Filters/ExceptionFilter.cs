using Contratos;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = StatusCodes.Status500InternalServerError;

        var response = new ResponseDto
        {
            ExecutionSuccessful = false,
            Message = "Internal server error"
        };

        if (context.Exception is UserValidationException ex)
        {
            statusCode = StatusCodes.Status400BadRequest;
            response.Message = ex.Message;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
