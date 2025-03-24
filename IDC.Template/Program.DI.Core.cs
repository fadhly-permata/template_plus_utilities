using IDC.Template.Utilities.DI;
using IDC.Utilities;

internal partial class Program
{
    /// <summary>
    /// Configures application configurations and registers them as a singleton service.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    /// <returns>An instance of <see cref="AppConfigsHandler"/>.</returns>
    /// <remarks>
    /// This method loads application configurations using <see cref="AppConfigsHandler.Load()"/>,
    /// adds them as a singleton service to the dependency injection container,
    /// and returns the loaded configurations.
    /// </remarks>
    /// <example>
    /// <code>
    /// var appConfigs = ConfigureAppConfigs(builder);
    /// </code>
    /// </example>
    private static AppConfigsHandler ConfigureAppConfigs(WebApplicationBuilder builder)
    {
        var appConfigs = AppConfigsHandler.Load();
        builder.Services.AddSingleton(_ => appConfigs);
        return appConfigs;
    }

    /// <summary>
    /// Configures application settings and registers them as a singleton service.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    /// <returns>An instance of <see cref="AppSettingsHandler"/>.</returns>
    /// <remarks>
    /// This method loads application settings using <see cref="AppSettingsHandler.Load()"/>,
    /// adds them as a singleton service to the dependency injection container,
    /// and returns the loaded settings.
    /// </remarks>
    /// <example>
    /// <code>
    /// var appSettings = ConfigureAppSettings(builder);
    /// </code>
    /// </example>
    private static AppSettingsHandler ConfigureAppSettings(WebApplicationBuilder builder)
    {
        var appSettings = AppSettingsHandler.Load();
        builder.Services.AddSingleton(_ => appSettings);
        return appSettings;
    }

    /// <summary>
    /// Configures language settings and registers them as a singleton service.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    /// <remarks>
    /// This method initializes a new <see cref="Language"/> instance with the specified JSON path
    /// and default language from application configurations. It then registers this instance
    /// as a singleton service in the dependency injection container.
    /// </remarks>
    /// <example>
    /// <code>
    /// ConfigureLanguage(builder);
    /// </code>
    /// </example>
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
