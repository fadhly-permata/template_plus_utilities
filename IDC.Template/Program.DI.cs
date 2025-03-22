using IDC.Template.Utilities.Middlewares;

internal partial class Program
{
    private static void SetupDI(WebApplicationBuilder builder)
    {
        ConfigureLanguage(builder: builder);
        ConfigureSwagger(builder: builder);
        ConfigureSystemLogging(builder: builder);
        ConfigureCaching(builder: builder);
        ConfigureSQLite(builder: builder);
        ConfigureMongoDB(builder: builder);

        builder.Services.AddScoped<ApiKeyAuthenticationMiddleware>();
    }
}
