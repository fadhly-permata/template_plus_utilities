using System.IO.Compression;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for compressing HTTP responses based on client's Accept-Encoding header.
/// </summary>
/// <remarks>
/// This middleware checks the client's Accept-Encoding header and applies either gzip or deflate compression
/// to the response body if supported. It helps reduce bandwidth usage and improve response times.
///
/// Example usage in Startup.cs:
/// <code>
/// app.UseMiddleware&lt;ResponseCompressionMiddleware&gt;();
/// </code>
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public class ResponseCompressionMiddleware(RequestDelegate next)
{
    private static readonly string[] CompressibleTypes =
    [
        "text/",
        "application/json",
        "application/xml",
        "application/javascript",
        "application/swagger+json",
        "application/swagger-json"
    ];

    /// <summary>
    /// Processes an individual HTTP request, applying compression if applicable.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var acceptEncoding = context.Request.Headers.AcceptEncoding.ToString().ToLower();

        if (string.IsNullOrEmpty(acceptEncoding))
        {
            await next(context);
            return;
        }

        await using var memoryStream = new MemoryStream();
        var originalBody = context.Response.Body;
        context.Response.Body = memoryStream;

        try
        {
            await next(context);

            if (
                context.Response.StatusCode == StatusCodes.Status204NoContent
                || context.Response.StatusCode == StatusCodes.Status304NotModified
            )
            {
                return;
            }

            var contentType = context.Response.ContentType?.ToLower() ?? string.Empty;
            var shouldCompress = CompressibleTypes.Any(type => contentType.Contains(type));

            if (!shouldCompress || memoryStream.Length == 0)
            {
                context.Response.Body = originalBody;
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(context.Response.Body);
                return;
            }

            context.Response.Headers.Remove("Content-Length");
            memoryStream.Position = 0;
            context.Response.Body = originalBody;

            if (acceptEncoding.Contains("gzip"))
            {
                context.Response.Headers.ContentEncoding = "gzip";
                await using var compressed = new GZipStream(
                    stream: context.Response.Body,
                    compressionLevel: CompressionLevel.Fastest,
                    leaveOpen: true
                );
                await memoryStream.CopyToAsync(compressed);
            }
            else if (acceptEncoding.Contains("deflate"))
            {
                context.Response.Headers.ContentEncoding = "deflate";
                await using var compressed = new DeflateStream(
                    stream: context.Response.Body,
                    compressionLevel: CompressionLevel.Fastest,
                    leaveOpen: true
                );
                await memoryStream.CopyToAsync(compressed);
            }
            else
            {
                await memoryStream.CopyToAsync(context.Response.Body);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}
