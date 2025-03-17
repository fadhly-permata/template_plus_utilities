using IDC.Template.Utilities.DI;
using IDC.Utilities;

internal partial class Program
{
    private static AppConfigsHandler _appConfigs = null!;
    private static AppSettingsHandler _appSettings = null!;
    private static SystemLogging _systemLogging = null!;
    private static Language _language = null!;

    private static void Main(string[] args)
    {
        // Increase file watcher limit for Linux systems
        if (OperatingSystem.IsLinux())
            Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");

        var builder = WebApplication.CreateBuilder(args: args);

        // Register Dependency Injections
        _appConfigs = ConfigureAppConfigs(builder: builder);
        _appSettings = ConfigureAppSettings(builder: builder);

        ConfigureServices(builder: builder);
        SetupDI(builder: builder);

        var app = builder.Build();

        ConfigureMiddlewares(app: app);

        if (_appConfigs.Get<bool>(path: "Security.Cors.Enabled"))
            app.UseCors("CorsPolicy");

        app.Run();
    }
}
