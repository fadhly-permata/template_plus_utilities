using IDC.Utilities;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for logging HTTP request details including execution time and status.
/// </summary>
/// <remarks>
/// This middleware logs:
/// - Request method and path
/// - Execution time in milliseconds
/// - Response status code
/// - Any errors that occur during request processing
///
/// Example:
/// <code>
/// app.UseMiddleware&lt;RequestLoggingMiddleware&gt;();
/// </code>
/// </remarks>
/// <param name="next">The next middleware in the pipeline</param>
/// <param name="systemLogging">The logging service for recording request information</param>
public class RequestLoggingMiddleware(RequestDelegate next, SystemLogging systemLogging)
{
    /// <summary>
    /// Processes an HTTP request and logs its execution details.
    /// </summary>
    /// <param name="context">The HTTP context for the request being processed.</param>
    /// <returns>A task representing the asynchronous middleware operation.</returns>
    /// <remarks>
    /// This method:
    /// 1. Records the start time
    /// 2. Executes the next middleware
    /// 3. Calculates and logs the execution time
    /// 4. Logs any errors that occur
    ///
    /// Example log output:
    /// "GET /api/values completed in 123.45ms with status 200"
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during request processing.</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await next(context);
            var elapsed = DateTime.UtcNow - start;

            systemLogging.LogInformation(
                $"{requestMethod} {requestPath} completed in {elapsed.TotalMilliseconds}ms with status {context.Response.StatusCode}"
            );
        }
        catch (Exception ex)
        {
            systemLogging.LogError($"{requestMethod} {requestPath} failed");
            systemLogging.LogError(ex);
            throw;
        }
    }
}
