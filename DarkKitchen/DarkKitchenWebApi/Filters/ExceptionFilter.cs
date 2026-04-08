using Contratos;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Exceptions;
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

        if (context.Exception is UserValidationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            response.Message = context.Exception.Message;
        }
        else if (context.Exception is UserAuthenticationException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            response.Message = context.Exception.Message;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
