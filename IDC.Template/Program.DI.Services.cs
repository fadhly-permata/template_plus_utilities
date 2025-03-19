using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Models.Data;

internal partial class Program
{
    /// <summary>
    /// Configures system logging for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure services for.</param>
    /// <remarks>
    /// This method sets up a singleton instance of <see cref="SystemLogging"/> with configuration
    /// parameters retrieved from application settings.
    ///
    /// Example:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// ConfigureSystemLogging(builder);
    /// </code>
    /// </remarks>
    private static void ConfigureSystemLogging(WebApplicationBuilder builder) =>
        builder.Services.AddSingleton(_ =>
        {
            _systemLogging = new SystemLogging(
                logDirectory: _appConfigs.Get(path: "Logging.LogDirectory", defaultValue: "logs"),
                source: _appConfigs.Get(path: "AppName", defaultValue: "IDC.Template"),
                enableOsLogging: _appConfigs.Get(path: "Logging.OSLogging", defaultValue: true),
                enableFileLogging: _appConfigs.Get(path: "Logging.FileLogging", defaultValue: true),
                autoCleanupOldLogs: _appConfigs.Get(
                    path: "Logging.AutoCleanupOldLogs",
                    defaultValue: true
                ),
                maxOldlogAge: _appConfigs.Get(path: "Logging.MaxOldlogAge", defaultValue: 30),
                baseDirectory: _appConfigs.Get(path: "Logging.BaseDirectory", defaultValue: ""),
                includeStackTrace: _appConfigs.Get(
                    path: "Logging.IncludeStackTrace",
                    defaultValue: true
                )
            );
            return _systemLogging;
        });

    /// <summary>
    /// Configures caching for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure services for.</param>
    /// <remarks>
    /// This method adds a singleton instance of <see cref="Caching"/> to the service collection
    /// if caching is enabled in the application configuration.
    ///
    /// Example:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// ConfigureCaching(builder);
    /// </code>
    /// </remarks>
    private static void ConfigureCaching(WebApplicationBuilder builder)
    {
        if (_appConfigs.Get(path: "DependencyInjection.Caching.Enable", defaultValue: false))
            builder.Services.AddSingleton(static _ => new Caching(
                defaultExpirationMinutes: _appConfigs.Get(
                    path: "DependencyInjection.Caching.ExpirationInMinutes",
                    defaultValue: 30
                )
            ));
    }

    /// <summary>
    /// Configures SQLite for the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure services for.</param>
    /// <remarks>
    /// This method adds a scoped instance of <see cref="SQLiteHelper"/> to the service collection
    /// if SQLite is enabled in the application configuration. It supports both in-memory and file-based SQLite databases.
    ///
    /// Example:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// ConfigureSQLite(builder);
    /// </code>
    /// </remarks>
    private static void ConfigureSQLite(WebApplicationBuilder builder)
    {
        if (_appConfigs.Get(path: "DependencyInjection.SQLite", defaultValue: false))
            builder.Services.AddScoped(static _ =>
            {
                string defaultConString = _appConfigs.Get(
                    path: "DefaultConStrings.SQLite",
                    defaultValue: "memory"
                );

                return defaultConString == "memory"
                    ? new SQLiteHelper(
                        logging: _appConfigs.Get(
                            path: "Logging.AttachToDIObjects",
                            defaultValue: true
                        )
                            ? (SystemLogging?)_systemLogging
                            : null
                    )
                    : new SQLiteHelper(
                        connectionString: new CommonConnectionString()
                            .FromConnectionString(
                                _appSettings.Get(
                                    path: $"SqLiteContextSettings.{defaultConString}",
                                    defaultValue: "memory"
                                )
                            )
                            .ToSQLite(),
                        logging: _appConfigs.Get(
                            path: "Logging.AttachToDIObjects",
                            defaultValue: true
                        )
                            ? _systemLogging
                            : null
                    );
            });
    }
}
