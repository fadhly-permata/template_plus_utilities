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
            var openApiInfo = new OpenApiInfo
            {
                Title = _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.Title")!,
                Version = _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.Version")!,
                Description = _appConfigs.Get<string>(
                    path: "SwaggerConfig.OpenApiInfo.Description"
                )!,
                TermsOfService = new Uri(
                    _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.TermsOfService")!,
                    UriKind.Relative
                ),
                Contact = new OpenApiContact
                {
                    Name = _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.Contact.Name")!,
                    Email = _appConfigs.Get<string>(
                        path: "SwaggerConfig.OpenApiInfo.Contact.Email"
                    )!,
                    Url = new Uri(
                        _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.Contact.Url")!
                    )
                },
                License = new OpenApiLicense
                {
                    Name = _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.License.Name")!,
                    Url = new Uri(
                        _appConfigs.Get<string>(path: "SwaggerConfig.OpenApiInfo.License.Url")!,
                        UriKind.Relative
                    )
                }
            };

            options.SwaggerDoc("Main", openApiInfo);
            options.SwaggerDoc(
                "Demo",
                new OpenApiInfo { Title = "Demo API", Version = openApiInfo.Version }
            );

            // Konfigurasi untuk mengelompokkan berdasarkan Tags
            options.TagActionsBy(api =>
                api.ActionDescriptor.EndpointMetadata.OfType<TagsAttribute>()
                    .SelectMany(attr => attr.Tags)
                    .Distinct()
                    .ToList()
            );

            // Urutkan Tags
            options.OrderActionsBy(apiDesc => apiDesc.GroupName);

            options.AddSecurityDefinition(
                "ApiKey",
                new OpenApiSecurityScheme
                {
                    Description = "API Key authentication using the 'X-API-Key' header",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-API-Key",
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );

            options.DocInclusionPredicate(
                (docName, api) =>
                {
                    if (docName == "Demo")
                        return api.RelativePath?.ToLower().Contains("/demo/") == true
                            || api.GroupName?.Equals("Demo", StringComparison.OrdinalIgnoreCase)
                                == true;

                    if (docName == "Main")
                        return api.GroupName?.Equals("Main", StringComparison.OrdinalIgnoreCase)
                                == true
                            || api.GroupName == null;

                    return true;
                }
            );

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

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
        app.UseSwaggerUI(options =>
        {
            // Main endpoints
            ConfigureMainEndpoint(options, _appConfigs);

            // Demo endpoints
            ConfigureDemoEndpoint(options);

            // Additional endpoints from SwaggerList
            ConfigureAdditionalEndpoints(options, app, _appConfigs);

            ConfigureSwaggerUIStyle(options);
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
            ";

        options.InjectJavascript("/js/swagger-theme-switcher.js");
    }
}
