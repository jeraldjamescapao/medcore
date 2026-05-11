namespace MedCore.Api.Middleware;

using MedCore.Api.Logging;
using MedCore.Common.Exceptions;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IProblemDetailsService problemDetailsService,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (OperationCanceledException) when (httpContext.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected...
            httpContext.Response.StatusCode = 499;
        }
        catch (DomainException domainException)
        {
            ApiLogMessages.DomainRuleViolation(
                _logger,
                httpContext.TraceIdentifier,
                httpContext.Request.Path.Value ?? string.Empty,
                domainException.Code,
                domainException);
            
            await WriteDomainProblemDetailsAsync(httpContext, domainException);
        }
        catch (Exception exception)
        {
            ApiLogMessages.UnhandledException(
                _logger,
                httpContext.TraceIdentifier,
                httpContext.Request.Path.Value ?? string.Empty,
                exception);

            await WriteUnhandledProblemDetailsAsync(httpContext, exception);
        }
    }

    private async Task WriteDomainProblemDetailsAsync(
        HttpContext httpContext, DomainException domainException)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = domainException,
            ProblemDetails =
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.21",
                Title = "Unprocessable Content",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = domainException.Message,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier,
                    ["code"] = domainException.Code
                }
            }
        });
    }

    private async Task WriteUnhandledProblemDetailsAsync(
        HttpContext httpContext, Exception exception)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier
                }
            }
        });
    }
}