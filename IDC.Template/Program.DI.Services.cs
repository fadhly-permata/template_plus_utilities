using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Models.Data;

internal partial class Program
{
    /// <summary>
    /// Configures system logging for the application.
    /// </summary>
    /// <param name="builder">The WebApplication builder instance.</param>
    /// <remarks>
    /// Configures a singleton instance of <see cref="SystemLogging"/> with the following capabilities:
    /// - File-based logging with configurable directory
    /// - Operating system logging (Event Log/Syslog)
    /// - Automatic cleanup of old logs
    /// - Stack trace inclusion
    ///
    /// Configuration is retrieved from appconfigs.jsonc:
    /// <code>
    /// {
    ///   "Logging": {
    ///     "LogDirectory": "logs",
    ///     "OSLogging": true,
    ///     "FileLogging": true,
    ///     "AutoCleanupOldLogs": true,
    ///     "MaxOldlogAge": 30,
    ///     "BaseDirectory": "",
    ///     "IncludeStackTrace": true
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="DirectoryNotFoundException">Thrown when log directory cannot be created or accessed.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when application lacks permission to write logs.</exception>
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
    /// Configures in-memory caching for the application.
    /// </summary>
    /// <param name="builder">The WebApplication builder instance.</param>
    /// <remarks>
    /// Adds a thread-safe singleton instance of <see cref="Caching"/> if enabled in configuration.
    ///
    /// Configuration from appconfigs.jsonc:
    /// <code>
    /// {
    ///   "DependencyInjection": {
    ///     "Caching": {
    ///       "Enable": true,
    ///       "ExpirationInMinutes": 30
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="OutOfMemoryException">Thrown when system lacks memory for cache operations.</exception>
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
    /// Configures SQLite database access for the application.
    /// </summary>
    /// <param name="builder">The WebApplication builder instance.</param>
    /// <remarks>
    /// Adds a scoped instance of <see cref="SQLiteHelper"/> if enabled in configuration.
    /// Supports both in-memory and file-based databases.
    ///
    /// Configuration from appconfigs.jsonc and appsettings.json:
    /// <code>
    /// // appconfigs.jsonc
    /// {
    ///   "DependencyInjection": {
    ///     "SQLite": true
    ///   },
    ///   "DefaultConStrings": {
    ///     "SQLite": "memory"
    ///   }
    /// }
    ///
    /// // appsettings.json
    /// {
    ///   "SqLiteContextSettings": {
    ///     "memory": "Data Source=:memory:",
    ///     "local": "Data Source=local.db"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="IOException">Thrown when database file operations fail.</exception>
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

    /// <summary>
    /// Configures MongoDB database access for the application.
    /// </summary>
    /// <param name="builder">The WebApplication builder instance.</param>
    /// <remarks>
    /// Adds a scoped instance of <see cref="MongoHelper"/> if enabled in configuration.
    ///
    /// Configuration from appconfigs.jsonc and appsettings.json:
    /// <code>
    /// // appconfigs.jsonc
    /// {
    ///   "DependencyInjection": {
    ///     "MongoDB": true
    ///   },
    ///   "DefaultConStrings": {
    ///     "MongoDB": "local"
    ///   }
    /// }
    ///
    /// // appsettings.json
    /// {
    ///   "MongoDBSettings": {
    ///     "local": "mongodb://localhost:27017/Learning",
    ///     "production": "mongodb://user:pass@host:27017/Learning"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="TimeoutException">Thrown when MongoDB connection times out.</exception>
    private static void ConfigureMongoDB(WebApplicationBuilder builder)
    {
        if (_appConfigs.Get(path: "DependencyInjection.MongoDB", defaultValue: false))
            builder.Services.AddScoped(static _ =>
            {
                string defaultConString = _appConfigs.Get(
                    path: "DefaultConStrings.MongoDB",
                    defaultValue: "local"
                );

                return new MongoHelper(
                    connectionString: _appSettings.Get(
                        path: $"MongoDBSettings.{defaultConString}",
                        defaultValue: "mongodb://localhost:27017/Learning"
                    ),
                    database: "Learning",
                    logging: _appConfigs.Get(path: "Logging.AttachToDIObjects", defaultValue: true)
                        ? _systemLogging
                        : null
                );
            });
    }
}
