using System.Reflection;
using IDC.Template.Utilities.DI;
using IDC.Template.Utilities.Middlewares.Swagger;
using IDC.Template.Utilities.Models;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

internal partial class Program
{
    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        if (_appConfigs.Get<bool>(path: "SwaggerConfig.UI.Enable") == false)
            return;

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                name: "Main",
                info: _appConfigs
                    .Get<JObject>(path: "SwaggerConfig.OpenApiInfo")
                    ?.ToObject<OpenApiInfo>()
                    ?? new OpenApiInfo
                    {
                        Title = _appConfigs.Get(path: "AppName", defaultValue: "IDC Template API"),
                        Version = "v1"
                    }
            );
            options.SwaggerDoc(
                name: "Demo",
                info: _appConfigs
                    .Get<JObject>(path: "SwaggerConfig.OpenApiInfo")
                    ?.ToObject<OpenApiInfo>()
                    ?? new OpenApiInfo { Title = "IDC Template Demo API", Version = "v1" }
            );

            var swaggerList = builder
                .Configuration.GetSection("SwaggerList")
                .Get<List<SwaggerEndpoint>>()
                ?.Where(endpoint =>
                    endpoint.Name
                        != _appConfigs.Get(path: "AppName", defaultValue: "IDC Template API")
                    && endpoint.Name != "IDC Template Demo API"
                );

            if (swaggerList != null)
            {
                foreach (var endpoint in swaggerList)
                {
                    options.SwaggerDoc(
                        name: endpoint.Name,
                        info: new OpenApiInfo { Title = endpoint.Name, Version = "v1" }
                    );
                }
            }

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(path1: AppContext.BaseDirectory, path2: xmlFile);
            options.IncludeXmlComments(filePath: xmlPath);

            options.DocumentFilter<DefaultGroupDocFilter>();
            if (_appConfigs.Get<bool>(path: "SwaggerConfig.UI.SortEndpoints"))
                options.DocumentFilter<DocumentSortDocFilter>();
        });
    }

    private static void ConfigureSwaggerUI(WebApplication app)
    {
        if (!_appConfigs.Get<bool>(path: "SwaggerConfig.UI.Enable"))
            return;

        app.UseSwagger();
        app.UseSwaggerUI(setupAction: options =>
        {
            ConfigureMainEndpoint(options: options, appConfigs: _appConfigs);
            ConfigureDemoEndpoint(options: options);
            ConfigureAdditionalEndpoints(options: options, app: app, appConfigs: _appConfigs);
            ConfigureSwaggerUIStyle(options: options);
        });
    }

    private static void ConfigureMainEndpoint(
        Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions options,
        AppConfigsHandler appConfigs
    ) =>
        options.SwaggerEndpoint(
            url: "/swagger/Main/swagger.json",
            name: appConfigs.Get(path: "AppName", defaultValue: "IDC Template API")
        );

    private static void ConfigureDemoEndpoint(
        Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions options
    ) => options.SwaggerEndpoint(url: "/swagger/Demo/swagger.json", name: "IDC Template Demo API");

    private static void ConfigureAdditionalEndpoints(
        Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions options,
        WebApplication app,
        AppConfigsHandler appConfigs
    )
    {
        var swaggerList = app
            .Configuration.GetSection("SwaggerList")
            .Get<List<SwaggerEndpoint>>()
            ?.Where(endpoint =>
                endpoint.Name != appConfigs.Get(path: "AppName", defaultValue: "IDC Template API")
                && endpoint.Name != "IDC Template Demo API"
            )
            .OrderBy(endpoint => endpoint.Name);

        if (swaggerList != null)
        {
            foreach (var endpoint in swaggerList)
            {
                options.SwaggerEndpoint(url: endpoint.URL, name: endpoint.Name);
            }
        }
    }

    private static void ConfigureSwaggerUIStyle(
        Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions options
    )
    {
        options.DocumentTitle =
            $"[SUI] {_appConfigs.Get(path: "AppName", defaultValue: "IDC Template API")}";

        options.InjectStylesheet(
            _appConfigs.Get(
                path: "SwaggerConfig.UI.Theme",
                defaultValue: "/themes/theme-monokai-dark.css"
            )!
        );
        options.InjectStylesheet("/_content/IDC.Template/css/swagger-custom.css");

        options.HeadContent =
            @"
                <link rel='stylesheet' type='text/css' href='/css/swagger-custom.css' />
                <link rel='shortcut icon' type='image/png' href='/images/logo_idecision2.png'/>
            ";

        options.InjectJavascript("/js/swagger-theme-switcher.js");
    }
}
