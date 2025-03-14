using IDC.Template.Utilities.DI;
using IDC.Template.Utilities.Middlewares;
using IDC.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

internal partial class Program
{
    private static AppConfigsHandler _appConfigs = null!;
    private static AppSettingsHandler _appSettings = null!;
    private static SystemLogging _systemLogging = null!;
    private static Language _language = null!;

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args: args);

        // Register Dependency Injections
        _appConfigs = ConfigureAppConfigs(builder: builder);
        _appSettings = ConfigureAppSettings(builder: builder);

        ConfigureServices(builder: builder);
        SetupDI(builder: builder);

        var app = builder.Build();

        ConfigureMiddlewares(app: app);
        app.UseCors("CorsPolicy");
        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder
            .Services.AddControllers(options =>
            {
                const string ContentType = "application/json";

                options.Filters.Add(filterType: typeof(ModelStateInvalidFilters));
                options.Filters.Add(item: new ConsumesAttribute(contentType: ContentType));
                options.Filters.Add(item: new ProducesAttribute(contentType: ContentType));
                options.Filters.Add(item: new ProducesResponseTypeAttribute(statusCode: 200));
                options.Filters.Add(
                    item: new ProducesResponseTypeAttribute(
                        type: typeof(APIResponseData<List<string>?>),
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
                options.Filters.Add(
                    item: new ProducesResponseTypeAttribute(
                        type: typeof(APIResponse),
                        statusCode: StatusCodes.Status500InternalServerError
                    )
                );
            })
            .AddNewtonsoftJson(setupAction: options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        // Add CORS policy
        if (_appConfigs.Get<bool>(path: "Cors.Enabled"))
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "CorsPolicy",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                _appConfigs.Get<string[]>(path: "Cors.AllowedHosts") ?? ["*"]
                            )
                            .WithHeaders(
                                _appConfigs.Get<string[]>(path: "Cors.AllowedHeaders") ?? ["*"]
                            )
                            .WithMethods(
                                _appConfigs.Get<string[]>(path: "Cors.AllowedMethods") ?? ["*"]
                            );
                    }
                );
            });
        }
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
