using IDC.Utilities;
using IDC.Utilities.Models.API;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for implementing rate limiting based on client IP address.
/// </summary>
/// <remarks>
/// Limits the number of requests from a single IP address within a specified time window.
///
/// The middleware uses memory cache to track request counts per IP address and enforces
/// a maximum limit within a rolling time window.
///
/// Example usage in Program.cs:
/// <code>
/// app.UseMiddleware&lt;RateLimitingMiddleware&gt;();
/// </code>
///
/// Example request handling:
/// <code>
/// // First request from IP 1.2.3.4
/// GET /api/data -> 200 OK
///
/// // After max requests exceeded
/// GET /api/data -> 429 Too Many Requests
/// </code>
/// </remarks>
/// <param name="next">The next middleware delegate in the pipeline</param>
/// <param name="cache">Memory cache service for storing request counts</param>
/// <param name="maxRequests">Maximum number of requests allowed per time window</param>
/// <param name="language">Language service for localized messages</param>
/// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limiting">Rate limiting in ASP.NET Core</seealso>
/// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory">Memory Cache in ASP.NET Core</seealso>
public class RateLimitingMiddleware(
    RequestDelegate next,
    Caching cache,
    int maxRequests,
    Language language
)
{
    /// <summary>
    /// Maximum number of allowed requests per time window
    /// </summary>
    /// <remarks>
    /// This value is injected through dependency injection and controls how many requests
    /// are allowed from a single IP address within the time window.
    /// </remarks>
    private readonly int MaxRequests = maxRequests;
    private readonly Language _language = language;

    /// <summary>
    /// Time window in minutes for rate limiting
    /// </summary>
    /// <remarks>
    /// Defines the rolling time window duration. Request counts are tracked within this window.
    /// After the window expires, the count resets.
    /// </remarks>
    private const int TimeWindowMinutes = 1;

    /// <summary>
    /// Processes HTTP request with rate limiting
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <remarks>
    /// This method:
    /// 1. Extracts client IP address from the request
    /// 2. Checks request count for the IP in cache
    /// 3. Returns 429 if limit exceeded
    /// 4. Increments count and continues pipeline if within limit
    ///
    /// Example cache key format:
    /// <code>
    /// ratelimit_192.168.1.1
    /// </code>
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions from next middleware</exception>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/429">HTTP 429 Too Many Requests</seealso>
    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"ratelimit_{ip}";

        var requestCount = cache.Get<int>(key: cacheKey, expirationRenewal: false);

        if (requestCount >= MaxRequests)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(
                new APIResponse()
                    .ChangeStatus(language: _language, key: "api.status.failed")
                    .ChangeMessage(language: _language, key: "api.rate_limit_exceeded")
            );
            return;
        }

        cache.Set(key: cacheKey, value: requestCount + 1, expirationMinutes: TimeWindowMinutes);
        await next(context);
    }
}
