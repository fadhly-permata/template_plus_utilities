using IDC.Template.Utilities.DI;
using IDC.Utilities;

internal partial class Program
{
    private static AppConfigsHandler ConfigureAppConfigs(WebApplicationBuilder builder)
    {
        var appConfigs = AppConfigsHandler.Load();
        builder.Services.AddSingleton(_ => appConfigs);
        return appConfigs;
    }

    private static AppSettingsHandler ConfigureAppSettings(WebApplicationBuilder builder)
    {
        var appSettings = AppSettingsHandler.Load();
        builder.Services.AddSingleton(_ => appSettings);
        return appSettings;
    }

    private static void ConfigureLanguage(WebApplicationBuilder builder)
    {
        _language = new Language(
            jsonPath: Path.Combine(
                path1: Directory.GetCurrentDirectory(),
                path2: "wwwroot/messages.json"
            ),
            defaultLanguage: _appConfigs.Get(path: "Language", defaultValue: "en")
        );

        builder.Services.AddSingleton(_ => _language);
    }
}
