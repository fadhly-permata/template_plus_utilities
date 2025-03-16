using IDC.Template.Utilities.Middlewares;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

internal partial class Program
{
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder
            .Services.AddControllers(options =>
            {
                const string ContentType = "application/json";

                options.Filters.Add(filterType: typeof(ModelStateInvalidFilters));
                options.Filters.Add(filterType: typeof(ExceptionHandlerFilter));
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
                        type: typeof(APIResponseData<List<string>?>),
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
        if (_appConfigs.Get<bool>(path: "Security.Cors.Enabled"))
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "CorsPolicy",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                _appConfigs.Get<string[]>(path: "Security.Cors.AllowedHosts")
                                    ?? ["*"]
                            )
                            .WithHeaders(
                                _appConfigs.Get<string[]>(path: "Security.Cors.AllowedHeaders")
                                    ?? ["*"]
                            )
                            .WithMethods(
                                _appConfigs.Get<string[]>(path: "Security.Cors.AllowedMethods")
                                    ?? ["*"]
                            );
                    }
                );
            });
        }
    }
}
