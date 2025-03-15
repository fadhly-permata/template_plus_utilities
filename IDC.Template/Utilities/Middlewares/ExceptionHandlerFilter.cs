using IDC.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Filter that handles exceptions and returns appropriate error responses.
/// </summary>
/// <remarks>
/// This filter implements IExceptionFilter to catch unhandled exceptions and convert them to standardized API responses.
/// When an exception occurs, it returns either a BadRequest (400) or InternalServerError (500) response with localized error messages.
///
/// Example response:
/// ```json
/// {
///   "status": "Failed",
///   "message": "An error occurred while processing your request",
///   "data": ["Detailed error message"]
/// }
/// ```
/// </remarks>
/// <example>
/// <code>
/// builder.Services.AddControllers(options =>
/// {
///     options.Filters.Add(typeof(ExceptionHandlerFilter));
/// });
/// </code>
/// </example>
public class ExceptionHandlerFilter(Language language, SystemLogging systemLogging)
    : IExceptionFilter
{
    /// <summary>
    /// Handles exceptions that occur during request processing.
    /// </summary>
    /// <param name="context">The exception context containing information about the error.</param>
    /// <remarks>
    /// Processes the exception and generates a standardized error response:
    /// - For BadRequest exceptions, returns a 400 status code
    /// - For other exceptions, returns a 500 status code
    /// - Includes localized error messages and stack traces in debug mode
    /// </remarks>
    public void OnException(ExceptionContext context)
    {
        var response = new APIResponseData<List<string>?>()
            .ChangeStatus(language: language, key: "api.status.failed")
            .ChangeMessage(
                exception: context.Exception,
                logging: systemLogging,
                includeStackTrace: Commons.IsDebugEnvironment()
            );

        context.Result = context.Exception switch
        {
            BadHttpRequestException => new BadRequestObjectResult(response),
            UnauthorizedAccessException => new UnauthorizedObjectResult(response),
            KeyNotFoundException => new NotFoundObjectResult(response),
            InvalidOperationException => new BadRequestObjectResult(response),
            ArgumentException => new BadRequestObjectResult(response),
            NotImplementedException => new StatusCodeResult(StatusCodes.Status501NotImplemented),
            TimeoutException => new StatusCodeResult(StatusCodes.Status408RequestTimeout),
            OperationCanceledException => new StatusCodeResult(StatusCodes.Status409Conflict),
            _
                => new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
        };
    }
}
