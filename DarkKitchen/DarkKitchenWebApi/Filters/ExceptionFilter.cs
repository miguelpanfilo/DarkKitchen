using BusinessLogicsAbstractions.Exceptions;
using Contract.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    private const string InternalErrorMessage = "Ocurrió un error interno.";

    public void OnException(ExceptionContext context)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = InternalErrorMessage;

        if (context.Exception is ValidationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = context.Exception.Message;
        }
        else if (context.Exception is UserAuthenticationException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            message = context.Exception.Message;
        }
        else if (context.Exception is ForbiddenException)
        {
            statusCode = StatusCodes.Status403Forbidden;
            message = context.Exception.Message;
        }
        else if (context.Exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            message = context.Exception.Message;
        }
        else if (context.Exception is ConflictException)
        {
            statusCode = StatusCodes.Status409Conflict;
            message = context.Exception.Message;
        }

        var response = new ResponseDto
        {
            ExecutionSuccessful = false,
            Message = message
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
