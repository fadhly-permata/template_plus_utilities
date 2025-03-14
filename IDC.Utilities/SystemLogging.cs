using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using IDC.Utilities.Extensions;
using IDC.Utilities.Models;

namespace IDC.Utilities;

/// <summary>
/// Handles system-wide logging functionality with support for multiple logging destinations.
/// </summary>
/// <remarks>
/// Provides logging capabilities to:
/// - Windows Event Log (on Windows systems)
/// - Syslog (on Unix-like systems)
/// - File system (daily rotating log files)
///
/// Features:
/// - Automatic old log cleanup
/// - Multiple log levels (Information, Warning, Error)
/// - Exception logging with stack traces
/// - Configurable logging destinations
///
/// Example:
/// ```csharp
/// using var logging = new SystemLogging(
///     logDirectory: "logs",
///     source: "MyApp",
///     enableOsLogging: true,
///     enableFileLogging: true
/// );
///
/// logging.LogInformation("Application started");
/// try
/// {
///     // Some code
/// }
/// catch (Exception ex)
/// {
///     logging.LogError(ex);
/// }
/// ```
/// </remarks>
/// <seealso cref="IDisposable"/>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog">EventLog Class</seealso>
/// <seealso href="https://en.wikipedia.org/wiki/Syslog">Syslog Protocol</seealso>
public sealed class SystemLogging : IDisposable
{
    /// <summary>Directory where log files are stored</summary>
    private readonly string _logDirectory;

    /// <summary>Whether to enable operating system logging (EventLog/Syslog)</summary>
    private readonly bool _enableOsLogging;

    /// <summary>Whether to enable file-based logging</summary>
    private readonly bool _enableFileLogging;

    /// <summary>Windows EventLog instance for Windows systems</summary>
    private readonly EventLog? _windowsEventLog;

    /// <summary>Source identifier for log entries</summary>
    private readonly string _source;

    /// <summary>Whether to automatically cleanup old log files</summary>
    private readonly bool _autoCleanupOldLogs;

    /// <summary>Maximum age in days for log files before cleanup</summary>
    private readonly int _maxOldlogAge;

    /// <summary>Base directory for log file storage</summary>
    private readonly string _baseDirectory;

    /// <summary>Whether to include stack traces in exception logs</summary>
    private readonly bool _includeStackTrace;

    /// <summary>Tracks whether Dispose has been called</summary>
    private bool _disposed;

    /// <summary>Default Windows Event Log name</summary>
    private const string DefaultLogName = "Application";

    /// <summary>
    /// Disposes the SystemLogging instance and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _windowsEventLog?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of SystemLogging.
    /// </summary>
    /// <param name="logDirectory">Directory path for log files</param>
    /// <param name="source">Source identifier for log entries</param>
    /// <param name="enableOsLogging">Enable OS-level logging</param>
    /// <param name="enableFileLogging">Enable file-based logging</param>
    /// <param name="autoCleanupOldLogs">Enable automatic cleanup of old logs</param>
    /// <param name="baseDirectory">Base directory for log storage</param>
    /// <param name="includeStackTrace">Include stack traces in exception logs</param>
    /// <param name="maxOldlogAge">Maximum age of log files in days</param>
    /// <exception cref="System.Security.SecurityException">Thrown when lacking permissions to create event source</exception>
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
        _baseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
            ? Directory.GetCurrentDirectory()
            : baseDirectory;
        _includeStackTrace = includeStackTrace;

        if (_enableOsLogging && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!EventLog.SourceExists(_source))
                EventLog.CreateEventSource(_source, DefaultLogName);
            _windowsEventLog = new(DefaultLogName, ".", _source);
        }

        if (_autoCleanupOldLogs)
            CleanupOldLogs();
    }

    /// <summary>Gets the full path to the log directory</summary>
    /// <returns>Absolute path to log directory</returns>
    private string GetLogPath() =>
        Path.GetFullPath(Path.Combine(_baseDirectory, _logDirectory.TrimStart('/', '\\')));

    /// <summary>Removes log files older than MaxOldLogAge days</summary>
    private void CleanupOldLogs()
    {
        try
        {
            var logPath = GetLogPath();
            if (!Directory.Exists(logPath))
                return;

            var now = DateTime.Now;
            var files = Directory.GetFiles(logPath, "logs-*.txt");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var age = (now - fileInfo.LastWriteTime).Days;

                if (age > _maxOldlogAge)
                    File.Delete(file);
            }
        }
        catch { }
    }

    /// <summary>Logs an informational message</summary>
    /// <param name="message">Message to log</param>
    public void LogInformation(string message) => Log(message, LogLevel.Information);

    /// <summary>Logs a warning message</summary>
    /// <param name="message">Message to log</param>
    public void LogWarning(string message) => Log(message, LogLevel.Warning);

    /// <summary>Logs an error message</summary>
    /// <param name="message">Message to log</param>
    public void LogError(string message) => Log(message, LogLevel.Error);

    /// <summary>Logs an exception as error with optional stack trace</summary>
    /// <param name="exception">Exception to log</param>
    public void LogError(Exception exception) =>
        LogError(exception.GetExceptionDetails(includeStackTrace: _includeStackTrace));

    /// <summary>Logs an exception as warning with optional stack trace</summary>
    /// <param name="exception">Exception to log</param>
    public void LogWarning(Exception exception) =>
        LogWarning(exception.GetExceptionDetails(includeStackTrace: _includeStackTrace));

    /// <summary>Logs an exception as information with optional stack trace</summary>
    /// <param name="exception">Exception to log</param>
    public void LogInformation(Exception exception) =>
        LogInformation(exception.GetExceptionDetails(includeStackTrace: _includeStackTrace));

    /// <summary>Core logging implementation</summary>
    /// <param name="message">Message to log</param>
    /// <param name="level">Log level</param>
    private void Log(string message, LogLevel level)
    {
        if (_disposed)
            return;

        if (_enableOsLogging)
        {
            LogToOperatingSystem(message, level);
        }

        if (_enableFileLogging)
        {
            LogToFile(message, level);
        }
    }

    /// <summary>Routes logging to appropriate OS logging system</summary>
    /// <param name="message">Message to log</param>
    /// <param name="level">Log level</param>
    private void LogToOperatingSystem(string message, LogLevel level)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LogToWindowsEvent(message, level);
        }
        else
        {
            LogToSyslog(message, level);
        }
    }

    /// <summary>Logs to Windows Event Log</summary>
    /// <param name="message">Message to log</param>
    /// <param name="level">Log level</param>
    private static void LogToWindowsEvent(string message, LogLevel level)
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

        _windowsEventLog.WriteEntry(message, eventLogEntryType);
#endif
    }

    /// <summary>
    /// Logs to Syslog daemon on Unix-like systems
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="level">Log level</param>
    /// <remarks>
    /// Uses UDP protocol to send messages to local syslog daemon on port 514.
    /// Follows RFC 3164 format: &lt;priority&gt;timestamp hostname tag: message
    /// </remarks>
    /// <seealso href="https://tools.ietf.org/html/rfc3164">RFC 3164 - The BSD syslog Protocol</seealso>
    private void LogToSyslog(string message, LogLevel level)
    {
        var facility = 1;
        var priority = level switch
        {
            LogLevel.Error => 3,
            LogLevel.Warning => 4,
            _ => 6
        };

        var timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
        var hostname = Environment.MachineName;
        var syslogMessage =
            $"<{facility * 8 + priority}>{timestamp} {hostname} {_source}: {message}";

        try
        {
            using var client = new System.Net.Sockets.UdpClient();
            var bytes = Encoding.UTF8.GetBytes(syslogMessage);
            client.Send(bytes, bytes.Length, "localhost", 514);
        }
        catch { }
    }

    /// <summary>
    /// Logs message to daily rotating log file
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="level">Log level</param>
    /// <remarks>
    /// Creates log files in format: logs-YYYYMMDD.txt
    /// Each entry format: [YYYY-MM-DD HH:mm:ss] [LEVEL] message
    /// </remarks>
    private void LogToFile(string message, LogLevel level)
    {
        var logPath = GetLogPath();
        var logFile = Path.Combine(logPath, $"logs-{DateTime.Now:yyyyMMdd}.txt");
        var logMessage =
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

        try
        {
            Directory.CreateDirectory(logPath);
            File.AppendAllText(logFile, logMessage);
        }
        catch { }
    }
}
