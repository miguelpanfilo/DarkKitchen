using Contratos;
using Contratos.Users;
using LogicaDeNegocioAbstracciones.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    private const string InternalErrorMessage = "Ocurrió un error interno.";
    private const string ValidationErrorMessage = "Los datos ingresados no son válidos.";
    private const string AuthenticationErrorMessage = "No fue posible autenticar al usuario.";
    private const string ForbiddenErrorMessage = "No tienes permisos para realizar esta acción.";
    private const string NotFoundErrorMessage = "No se encontró el recurso solicitado.";
    private const string ConflictErrorMessage = "No fue posible completar la operación por un conflicto de datos.";

    public void OnException(ExceptionContext context)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = InternalErrorMessage;

        if(context.Exception is ValidationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = ValidationErrorMessage;
        }
        else if(context.Exception is UserAuthenticationException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            message = AuthenticationErrorMessage;
        }
        else if(context.Exception is ForbiddenException)
        {
            statusCode = StatusCodes.Status403Forbidden;
            message = ForbiddenErrorMessage;
        }
        else if(context.Exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            message = NotFoundErrorMessage;
        }
        else if(context.Exception is IConflictException)
        {
            statusCode = StatusCodes.Status409Conflict;
            message = ConflictErrorMessage;
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
