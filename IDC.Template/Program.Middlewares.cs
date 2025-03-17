using IDC.Template.Utilities.Middlewares;
using Microsoft.Extensions.FileProviders;

internal partial class Program
{
    private static void ConfigureMiddlewares(WebApplication app)
    {
        if (_appConfigs.Get(path: "Middlewares.RequestLogging", defaultValue: true))
            app.UseMiddleware<RequestLoggingMiddleware>();

        if (_appConfigs.Get(path: "Middlewares.RateLimiting.Enabled", defaultValue: true))
            app.UseMiddleware<RateLimitingMiddleware>(
                _appConfigs.Get(
                    path: "Middlewares.RateLimiting.MaxRequestsPerMinute",
                    defaultValue: 100
                )
            );

        if (_appConfigs.Get(path: "Middlewares.ResponseCompression", defaultValue: true))
            app.UseMiddleware<ResponseCompressionMiddleware>();

        if (_appConfigs.Get(path: "Middlewares.SecurityHeaders.Enabled", defaultValue: true))
            app.UseMiddleware<SecurityHeadersMiddleware>(
                _appConfigs.Get(
                    path: "Middlewares.SecurityHeaders.EnableForHttp",
                    defaultValue: true
                ),
                _appConfigs.Get(
                    path: "Middlewares.SecurityHeaders.EnableForHttps",
                    defaultValue: true
                ),
                _appConfigs.Get(
                    path: "Middlewares.SecurityHeaders.EnableForAllEndpoints",
                    defaultValue: true
                ),
                _appConfigs.Get(
                    path: "Middlewares.SecurityHeaders.Options",
                    defaultValue: new Dictionary<string, string>()
                )
            );

        if (_appConfigs.Get(path: "Middlewares.ApiKeyAuthentication", defaultValue: true))
        {
            app.UseWhen(
                predicate: context =>
                {
                    var path = context.Request.Path.Value?.ToLower();
                    return path?.StartsWith("/swagger") != true
                        && path?.StartsWith("/css") != true
                        && path?.StartsWith("/js") != true
                        && path?.StartsWith("/themes") != true
                        && path?.StartsWith("/images") != true;
                },
                configuration: appBuilder =>
                {
                    appBuilder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
                }
            );
        }

        ConfigureSwaggerUI(app: app);
        app.UseHttpsRedirection();
        ConfigureStaticFiles(app: app);
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void ConfigureStaticFiles(WebApplication app)
    {
        app.UseStaticFiles(
            options: new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    root: Path.Combine(path1: Directory.GetCurrentDirectory(), path2: "wwwroot")
                ),
                RequestPath = ""
            }
        );
    }
}
