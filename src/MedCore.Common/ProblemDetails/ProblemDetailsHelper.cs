namespace MedCore.Common.ProblemDetails;

using MedCore.Common.Results;
using Microsoft.AspNetCore.Mvc;

public static class ProblemDetailsHelper
{
    private static readonly IReadOnlyDictionary<ResultErrorType, (int StatusCode, string Type, string Title)> Map =
        new Dictionary<ResultErrorType, (int, string, string)>
        {
            [ResultErrorType.Validation] = (
                400,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "Bad Request"),
            [ResultErrorType.Unauthorized] = (
                401,
                "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                "Unauthorized"),
            [ResultErrorType.Forbidden] = (
                403,
                "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                "Forbidden"),
            [ResultErrorType.NotFound] = (
                404,
                "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                "Not Found"),
            [ResultErrorType.Conflict] = (
                409,
                "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                "Conflict"),
            [ResultErrorType.UnprocessableEntity] = (
                422,
                "https://tools.ietf.org/html/rfc9110#section-15.5.21",
                "Unprocessable Content"),
            [ResultErrorType.Internal] = (
                500,
                "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                "Internal Server Error"),
            [ResultErrorType.ServiceUnavailable] = (
                503,
                "https://tools.ietf.org/html/rfc9110#section-15.6.4",
                "Service Unavailable"),
        };
    
    public static ProblemDetails FromResult(ResultError error, ResultErrorType errorType, string instance, string traceId)
    {
        var entry = GetEntry(errorType);
        
        return new ProblemDetails
        {
            Type = entry.Type,
            Title = entry.Title,
            Status = entry.StatusCode,
            Detail = error.Message,
            Instance = instance,
            Extensions =
            {
                ["traceId"] = traceId,
                ["code"] = error.Code
            }
        };
    }
    
    private static (int StatusCode, string Type, string Title) GetEntry(ResultErrorType errorType)
    {
        if (!Map.TryGetValue(errorType, out var entry))
            throw new InvalidOperationException($"Unhandled error type: {errorType}.");

        return entry;
    }
}