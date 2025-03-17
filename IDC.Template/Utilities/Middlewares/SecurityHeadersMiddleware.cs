namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for adding security headers to HTTP responses.
/// </summary>
/// <remarks>
/// This middleware adds various security headers to enhance the security of the application.
/// It sets headers such as X-Frame-Options, X-Content-Type-Options, X-XSS-Protection,
/// Referrer-Policy, Content-Security-Policy, and Permissions-Policy.
///
/// Example usage:
/// <code>
/// app.UseMiddleware&lt;SecurityHeadersMiddleware&gt;(
///     enableForHttp: true,
///     enableForHttps: true,
///     enableForAllEndpoints: true,
///     options: new Dictionary&lt;string, string&gt; {
///         ["X-Frame-Options"] = "DENY",
///         ["X-Content-Type-Options"] = "nosniff"
///     }
/// );
/// </code>
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="SecurityHeadersMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="enableForHttp">Whether to enable security headers for HTTP requests.</param>
/// <param name="enableForHttps">Whether to enable security headers for HTTPS requests.</param>
/// <param name="enableForAllEndpoints">Whether to enable security headers for all endpoints.</param>
/// <param name="options">Custom security header options.</param>
public class SecurityHeadersMiddleware(
    RequestDelegate next,
    bool enableForHttp,
    bool enableForHttps,
    bool enableForAllEndpoints,
    IDictionary<string, string> options
)
{
    private readonly RequestDelegate _next = next;
    private readonly bool _enableForHttp = enableForHttp;
    private readonly bool _enableForHttps = enableForHttps;
    private readonly bool _enableForAllEndpoints = enableForAllEndpoints;
    private readonly IDictionary<string, string> _options = options;

    /// <summary>
    /// Invokes the middleware to add security headers.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldApplyHeaders(context))
            foreach (var option in _options)
                context.Response.Headers[option.Key] = option.Value;

        await _next(context);
    }

    private bool ShouldApplyHeaders(HttpContext context)
    {
        if (!_enableForAllEndpoints && !context.Request.Path.StartsWithSegments("/api"))
            return false;

        return context.Request.IsHttps ? _enableForHttps : _enableForHttp;
    }
}
