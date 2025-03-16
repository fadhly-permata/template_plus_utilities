namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for implementing rate limiting based on client IP address.
/// </summary>
/// <remarks>
/// Limits the number of requests from a single IP address within a specified time window.
///
/// Example usage in Program.cs:
/// <code>
/// app.UseMiddleware&lt;RateLimitingMiddleware&gt;();
/// </code>
/// </remarks>
/// <param name="next">The next middleware in the pipeline</param>
/// <param name="cache">Memory cache for storing request counts</param>
/// <param name="maxRequests">Maximum number of requests allowed per time window</param>
/// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory">Memory Cache in ASP.NET Core</seealso>
public class RateLimitingMiddleware(
    RequestDelegate next,
    IDC.Utilities.Caching cache,
    int maxRequests
)
{
    /// <summary>
    /// Maximum number of allowed requests per time window
    /// </summary>
    private readonly int MaxRequests = maxRequests;

    /// <summary>
    /// Time window in minutes for rate limiting
    /// </summary>
    private const int TimeWindowMinutes = 1;

    /// <summary>
    /// Processes HTTP request with rate limiting
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <remarks>
    /// Tracks request count per IP address and returns 429 (Too Many Requests) when limit exceeded.
    /// Uses memory cache to store request counts with IP-based keys.
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions from next middleware</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"ratelimit_{ip}";

        var requestCount = cache.Get<int>(key: cacheKey);

        if (requestCount >= MaxRequests)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new { message = "Too many requests" });
            return;
        }

        cache.Set(key: cacheKey, value: requestCount + 1, expirationMinutes: TimeWindowMinutes);
        await next(context);
    }
}
