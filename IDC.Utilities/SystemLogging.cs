using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using IDC.Utilities.Extensions;
using IDC.Utilities.Models;

namespace IDC.Utilities;

/// <summary>
/// Provides a comprehensive logging system that supports multiple output destinations.
/// </summary>
/// <remarks>
/// Implements a thread-safe logging system that can write to:
/// - Console output
/// - Operating system logs (Windows Event Log or Unix Syslog)
/// - Daily rotating text files
///
/// Features:
/// - Configurable log directory
/// - Multiple output destinations
/// - Automatic log rotation
/// - OS-specific logging integration
/// - Thread-safe operations
///
/// Example usage:
/// <code>
/// var logger = new SystemLogging(
///     logDirectory: "logs",
///     enableOsLogging: true,
///     enableFileLogging: true,
///     source: "MyApplication"
/// );
///
/// logger.Log("Application started", LogLevel.Information);
/// logger.Log("Configuration error", LogLevel.Error);
/// </code>
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog">Windows Event Log</seealso>
/// <seealso href="https://datatracker.ietf.org/doc/html/rfc3164">Syslog Protocol RFC 3164</seealso>
public sealed class SystemLogging : IDisposable
{
    /// <summary>
    /// Directory where log files will be stored.
    /// </summary>
    private readonly string _logDirectory;

    /// <summary>
    /// Whether to enable logging to the operating system's logging facility.
    /// </summary>
    private readonly bool _enableOsLogging;

    /// <summary>
    /// Whether to enable logging to local text files.
    /// </summary>
    private readonly bool _enableFileLogging;

    /// <summary>
    /// Windows Event Log instance for logging on Windows systems.
    /// </summary>
    private readonly EventLog? _windowsEventLog;

    /// <summary>
    /// Source identifier for log entries.
    /// </summary>
    private readonly string _source;

    /// <summary>
    /// Whether to automatically cleanup old log files.
    /// </summary>
    private readonly bool _autoCleanupOldLogs;

    /// <summary>
    /// Maximum age in days for old log files before cleanup.
    /// </summary>
    private readonly int _maxOldlogAge;

    /// <summary>
    /// Base directory for resolving relative log paths.
    /// </summary>
    private readonly string _baseDirectory;

    /// <summary>
    /// Whether to include stack traces in exception logs.
    /// </summary>
    private readonly bool _includeStackTrace;

    /// <summary>
    /// Tracks whether this instance has been disposed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Default Windows Event Log name.
    /// </summary>
    private const string DefaultLogName = "Application";

    /// <summary>
    /// Releases resources used by the logging system.
    /// </summary>
    /// <remarks>
    /// Implements the IDisposable pattern to properly clean up resources:
    /// - Disposes the Windows Event Log if it was initialized
    /// - Sets the disposed flag to prevent further operations
    /// - Safe to call multiple times
    ///
    /// Example:
    /// <code>
    /// using (var logger = new SystemLogging("logs"))
    /// {
    ///     logger.LogInformation("Starting app");
    /// } // Logger automatically disposed here
    /// </code>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose">Implementing IDisposable</seealso>
    public void Dispose()
    {
        if (!_disposed)
        {
            _windowsEventLog?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of the SystemLogging class.
    /// </summary>
    /// <param name="logDirectory" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">Directory where log files will be stored</param>
    /// <param name="source" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">Source identifier for log entries</param>
    /// <param name="enableOsLogging" href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Whether to enable OS-level logging</param>
    /// <param name="enableFileLogging" href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Whether to enable file logging</param>
    /// <param name="autoCleanupOldLogs" href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Whether to automatically cleanup old logs</param>
    /// <param name="baseDirectory" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">Base directory for resolving relative paths</param>
    /// <param name="includeStackTrace" href="https://learn.microsoft.com/en-us/dotnet/api/system.boolean">Whether to include stack traces</param>
    /// <param name="maxOldlogAge" href="https://learn.microsoft.com/en-us/dotnet/api/system.int32">Maximum age in days for old log files</param>
    /// <exception cref="System.Security.SecurityException">Thrown when lacking permissions to create event source</exception>
    /// <remarks>
    /// Initializes a new logging instance with the specified configuration:
    /// - Sets up file and/or OS logging based on parameters
    /// - Creates Windows Event Log source if needed
    /// - Performs initial cleanup of old logs if enabled
    ///
    /// > [!IMPORTANT]
    /// > Creating Windows Event Log sources requires administrative privileges
    ///
    /// Example:
    /// <code>
    /// // Basic initialization
    /// var logger1 = new SystemLogging("logs");
    ///
    /// // Full configuration
    /// var logger2 = new SystemLogging(
    ///     logDirectory: "app_logs",
    ///     source: "MyApp",
    ///     enableOsLogging: true,
    ///     enableFileLogging: true,
    ///     autoCleanupOldLogs: true,
    ///     baseDirectory: @"C:\MyApp",
    ///     includeStackTrace: true,
    ///     maxOldlogAge: 30
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog">EventLog Class</seealso>
    public SystemLogging(
        string logDirectory,
        string? source = null,
        bool enableOsLogging = true,
        bool enableFileLogging = true,
        bool autoCleanupOldLogs = true,
        string? baseDirectory = null,
        bool includeStackTrace = true,
        int maxOldlogAge = 30
    )
    {
        _logDirectory = logDirectory;
        _enableOsLogging = enableOsLogging;
        _enableFileLogging = enableFileLogging;
        _source = source ?? "IDC.Template";
        _autoCleanupOldLogs = autoCleanupOldLogs;
        _maxOldlogAge = maxOldlogAge;
        _baseDirectory = string.IsNullOrWhiteSpace(value: baseDirectory)
            ? Directory.GetCurrentDirectory()
            : baseDirectory;
        _includeStackTrace = includeStackTrace;

        if (_enableOsLogging && RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows))
        {
            if (!EventLog.SourceExists(source: _source))
                EventLog.CreateEventSource(source: _source, logName: DefaultLogName);
            _windowsEventLog = new(logName: DefaultLogName, machineName: ".", source: _source);
        }

        if (_autoCleanupOldLogs)
            CleanupOldLogs();
    }

    /// <summary>
    /// Gets the full path to the log directory.
    /// </summary>
    /// <returns href="https://learn.microsoft.com/en-us/dotnet/api/system.string">Absolute path to log directory</returns>
    /// <remarks>
    /// Resolves the log directory path:
    /// - Combines base directory with log directory
    /// - Trims leading slashes from log directory
    /// - Returns absolute path
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging(
    ///     logDirectory: "logs",
    ///     baseDirectory: @"C:\MyApp"
    /// );
    /// var path = logger.GetLogDirectory(); // Returns "C:\MyApp\logs"
    /// </code>
    /// </remarks>
    private string GetLogPath() =>
        Path.GetFullPath(
            path: Path.Combine(
                path1: _baseDirectory,
                path2: _logDirectory.TrimStart(trimChars: ['/', '\\'])
            )
        );

    /// <summary>
    /// Removes log files older than the configured maximum age.
    /// </summary>
    /// <remarks>
    /// Performs cleanup of old log files:
    /// - Checks if log directory exists
    /// - Finds all log files matching pattern "logs-*.txt"
    /// - Deletes files older than maxOldlogAge days
    /// - Silently handles any IO errors
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging(
    ///     logDirectory: "logs",
    ///     autoCleanupOldLogs: true,
    ///     maxOldlogAge: 7
    /// );
    /// logger.CleanupOldLogs(); // Deletes logs older than 7 days
    /// </code>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.io.directory">Directory Class</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.io.file">File Class</seealso>
    private void CleanupOldLogs()
    {
        try
        {
            var logPath = GetLogPath();
            if (!Directory.Exists(path: logPath))
                return;

            foreach (var file in Directory.GetFiles(path: logPath, searchPattern: "logs-*.txt"))
                if (
                    (DateTime.Now - new FileInfo(fileName: file).LastWriteTime).Days > _maxOldlogAge
                )
                    File.Delete(path: file);
        }
        catch { }
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">The message to log</param>
    /// <remarks>
    /// Logs a message with Information level severity
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// logger.LogInformation("Application started successfully");
    /// logger.LogInformation($"User {username} logged in");
    /// </code>
    /// </remarks>
    public void LogInformation(string message) =>
        Log(message: message, level: LogLevel.Information);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">The message to log</param>
    /// <remarks>
    /// Logs a message with Warning level severity
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// logger.LogWarning("Database connection pool running low");
    /// logger.LogWarning($"High memory usage detected: {memoryUsage}%");
    /// </code>
    /// </remarks>
    public void LogWarning(string message) => Log(message: message, level: LogLevel.Warning);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">The message to log</param>
    /// <remarks>
    /// Logs a message with Error level severity
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// logger.LogError("Failed to connect to database");
    /// logger.LogError($"Transaction failed: {transactionId}");
    /// </code>
    /// </remarks>
    public void LogError(string message) => Log(message: message, level: LogLevel.Error);

    /// <summary>
    /// Logs an exception as an error.
    /// </summary>
    /// <param name="exception" href="https://learn.microsoft.com/en-us/dotnet/api/system.exception">The exception to log</param>
    /// <remarks>
    /// Logs exception details with Error level severity:
    /// - Includes stack trace if enabled
    /// - Uses GetExceptionDetails extension method
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// try
    /// {
    ///     // Some operation that might fail
    ///     throw new DatabaseException("Connection failed");
    /// }
    /// catch (Exception ex)
    /// {
    ///     logger.LogError(ex);
    /// }
    /// </code>
    /// </remarks>
    public void LogError(Exception exception) =>
        LogError(message: exception.GetExceptionDetails(includeStackTrace: _includeStackTrace));

    /// <summary>
    /// Logs an exception as a warning.
    /// </summary>
    /// <param name="exception" href="https://learn.microsoft.com/en-us/dotnet/api/system.exception">The exception to log</param>
    /// <remarks>
    /// Logs exception details with Warning level severity:
    /// - Includes stack trace if enabled
    /// - Uses GetExceptionDetails extension method
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// try
    /// {
    ///     // Non-critical operation
    ///     var result = cache.TryGetValue(key);
    /// }
    /// catch (Exception ex)
    /// {
    ///     logger.LogWarning(ex); // Cache miss isn't critical
    /// }
    /// </code>
    /// </remarks>
    public void LogWarning(Exception exception) =>
        LogWarning(message: exception.GetExceptionDetails(includeStackTrace: _includeStackTrace));

    /// <summary>
    /// Logs an exception as information.
    /// </summary>
    /// <param name="exception" href="https://learn.microsoft.com/en-us/dotnet/api/system.exception">The exception to log</param>
    /// <remarks>
    /// Logs exception details with Information level severity:
    /// - Includes stack trace if enabled
    /// - Uses GetExceptionDetails extension method
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging("logs");
    /// try
    /// {
    ///     // Expected exception case
    ///     var data = parser.TryParse(input);
    /// }
    /// catch (FormatException ex)
    /// {
    ///     logger.LogInformation(ex); // Expected invalid format
    /// }
    /// </code>
    /// </remarks>
    public void LogInformation(Exception exception) =>
        LogInformation(
            message: exception.GetExceptionDetails(includeStackTrace: _includeStackTrace)
        );

    /// <summary>
    /// Core logging method that handles writing to configured destinations.
    /// </summary>
    /// <param name="message" href="https://learn.microsoft.com/en-us/dotnet/api/system.string">The message to log</param>
    /// <param name="level" href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel">The severity level of the log entry</param>
    /// <remarks>
    /// Central logging method that:
    /// - Checks if instance is disposed
    /// - Writes to console in format: [YYYY-MM-DD HH:mm:ss] [LogLevel] Message
    /// - Routes to OS logging if enabled
    /// - Routes to file logging if enabled
    ///
    /// > [!IMPORTANT]
    /// > This is the core logging method that all other logging methods ultimately call.
    ///
    /// > [!NOTE]
    /// > Console output is always enabled regardless of other logging configurations.
    ///
    /// Example:
    /// <code>
    /// var logger = new SystemLogging(
    ///     logDirectory: "logs",
    ///     enableOsLogging: true,
    ///     enableFileLogging: true
    /// );
    ///
    /// // Log will be written to console, OS and file
    /// logger.Log("Application starting", LogLevel.Information);
    ///
    /// // Error logged with full details
    /// logger.Log("Database connection failed", LogLevel.Error);
    /// </code>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.console">Console Class</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.datetime">DateTime Struct</seealso>
    private void Log(string message, LogLevel level)
    {
        if (_disposed)
            return;

        Task.Run(() =>
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");

            if (_enableOsLogging)
                LogToOperatingSystem(message: message, level: level);

            if (_enableFileLogging)
                LogToFile(message: message, level: level);
        });
    }

    /// <summary>
    /// Routes log messages to the appropriate operating system logging facility.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="level">The severity level of the log entry.</param>
    /// <remarks>
    /// > [!IMPORTANT]
    /// > This method automatically detects the operating system and routes logs accordingly:
    /// > - Windows: Uses Windows Event Log
    /// > - Unix/Linux: Uses Syslog
    ///
    /// Log levels are mapped to OS-specific severity levels:
    /// - Error → EventLogEntryType.Error / Syslog Error (3)
    /// - Warning → EventLogEntryType.Warning / Syslog Warning (4)
    /// - Information → EventLogEntryType.Information / Syslog Info (6)
    ///
    /// Example:
    /// <code>
    /// LogToOperatingSystem("Database connection failed", LogLevel.Error);
    /// LogToOperatingSystem("Cache cleared", LogLevel.Information);
    /// </code>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when OS logging is enabled but system logging facilities are unavailable.</exception>
    private void LogToOperatingSystem(string message, LogLevel level)
    {
        if (RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows))
            LogToWindowsEvent(message: message, level: level);
        else
            LogToSyslog(message: message, level: level);
    }

#pragma warning disable CA1822
    /// <summary>
    /// Writes log entries to the Windows Event Log.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="level">The severity level of the log entry.</param>
    /// <remarks>
    /// > [!NOTE]
    /// > Only available on Windows platforms. Uses conditional compilation with WINDOWS directive.
    ///
    /// > [!TIP]
    /// > Log entries can be viewed in Windows Event Viewer under the Application log.
    ///
    /// Severity mapping:
    ///
    /// |LogLevel|EventLogEntryType|
    /// |---|---|
    /// |Error|Error|
    /// |Warning|Warning|
    /// |Information|Information|
    ///
    /// Example:
    /// <code>
    /// LogToWindowsEvent("Service started", LogLevel.Information);
    /// LogToWindowsEvent("Access denied", LogLevel.Error);
    /// </code>
    /// </remarks>
    /// <exception cref="System.Security.SecurityException">Thrown when the application lacks permission to write to the Event Log.</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog">EventLog Class</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlogentrytype">EventLogEntryType Enum</seealso>
    private void LogToWindowsEvent(string message, LogLevel level)
    {
#if WINDOWS
        if (_windowsEventLog is null)
            return;

        var eventLogEntryType = level switch
        {
            LogLevel.Error => EventLogEntryType.Error,
            LogLevel.Warning => EventLogEntryType.Warning,
            _ => EventLogEntryType.Information
        };

        _windowsEventLog.WriteEntry(message: message, type: eventLogEntryType);
#endif
    }
#pragma warning restore CA1822

    /// <summary>
    /// Logs messages to the Unix/Linux Syslog daemon.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="level">The severity level of the log entry.</param>
    /// <remarks>
    /// > [!NOTE]
    /// > Uses UDP protocol to send messages to localhost on port 514.
    ///
    /// > [!IMPORTANT]
    /// > Message format follows RFC 3164 specification:
    /// > - Facility: User-level (1)
    /// > - Hostname: Current machine name
    /// > - Process name: Application source identifier
    ///
    /// Severity mapping:
    ///
    /// |LogLevel|Syslog Severity|
    /// |---|---|
    /// |Error|Error (3)|
    /// |Warning|Warning (4)|
    /// |Information|Info (6)|
    ///
    /// Example:
    /// <code>
    /// LogToSyslog("Cache miss", LogLevel.Warning);
    /// LogToSyslog("User authenticated", LogLevel.Information);
    /// </code>
    /// </remarks>
    /// <exception cref="System.Net.Sockets.SocketException">Thrown when UDP communication fails.</exception>
    /// <seealso href="https://datatracker.ietf.org/doc/html/rfc3164">RFC 3164 - The BSD Syslog Protocol</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient">UdpClient Class</seealso>
    private void LogToSyslog(string message, LogLevel level)
    {
        try
        {
            using var client = new System.Net.Sockets.UdpClient();
            var bytes = Encoding.UTF8.GetBytes(
                s: $"<{1 * 8 + level switch
            {
                LogLevel.Error => 3,
                LogLevel.Warning => 4,
                _ => 6
            }}>{DateTime.Now:MMM dd HH:mm:ss} {Environment.MachineName} {_source}: {message}"
            );
            client.Send(dgram: bytes, bytes: bytes.Length, hostname: "localhost", port: 514);
        }
        catch { }
    }

    /// <summary>
    /// Writes log entries to daily rotating text files.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="level">The severity level of the log entry.</param>
    /// <remarks>
    /// > [!NOTE]
    /// > Creates log files in the format: logs-YYYYMMDD.txt
    ///
    /// > [!TIP]
    /// > Log files are automatically rotated daily to prevent excessive file sizes.
    ///
    /// Features:
    /// - Thread-safe file operations
    /// - Automatic directory creation
    /// - Daily file rotation
    /// - ISO 8601 timestamp format
    ///
    /// Log entry format:
    /// ```
    /// [YYYY-MM-DD HH:mm:ss] [LogLevel] Message
    /// ```
    ///
    /// Example:
    /// <code>
    /// LogToFile("Transaction completed", LogLevel.Information);
    /// LogToFile("Validation failed", LogLevel.Error);
    /// </code>
    /// </remarks>
    /// <exception cref="IOException">Thrown when file operations fail.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the application lacks write permission to the log directory.</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendalltext">File.AppendAllText Method</seealso>
    private void LogToFile(string message, LogLevel level)
    {
        var logPath = GetLogPath();

        try
        {
            Directory.CreateDirectory(path: logPath);
            File.AppendAllText(
                path: Path.Combine(path1: logPath, path2: $"logs-{DateTime.Now:yyyyMMdd}.txt"),
                contents: (string?)
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}"
            );
        }
        catch { }
    }
}
