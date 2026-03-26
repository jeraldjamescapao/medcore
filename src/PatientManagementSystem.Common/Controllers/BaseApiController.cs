using Microsoft.AspNetCore.Http;

namespace PatientManagementSystem.Common.Controllers;

using Microsoft.AspNetCore.Mvc;
using PatientManagementSystem.Common.Results;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess) 
            return StatusCode(StatusCodes.Status200OK, result.Value);

        var error = new
        {
            code = result.Error!.Code,
            message = result.Error.Message
        };
        
        var objectResult =  result.ErrorType switch
        {
            ResultErrorType.Validation          => StatusCode(StatusCodes.Status400BadRequest, error),
            ResultErrorType.Unauthorized        => StatusCode(StatusCodes.Status401Unauthorized, error),
            ResultErrorType.Forbidden           => StatusCode(StatusCodes.Status403Forbidden, error),
            ResultErrorType.NotFound            => StatusCode(StatusCodes.Status404NotFound, error),
            ResultErrorType.Conflict            => StatusCode(StatusCodes.Status409Conflict, error),
            ResultErrorType.UnprocessableEntity => StatusCode(StatusCodes.Status422UnprocessableEntity, error),
            ResultErrorType.Internal            => StatusCode(StatusCodes.Status500InternalServerError, error),
            ResultErrorType.ServiceUnavailable  => StatusCode(StatusCodes.Status503ServiceUnavailable, error),
            _ => throw new InvalidOperationException($"Unhandled error type: {result.ErrorType}.")
        };
        
        return objectResult;
    }
}