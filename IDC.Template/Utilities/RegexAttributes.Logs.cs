using System.Text.RegularExpressions;

namespace IDC.Template.Utilities;

/// <summary>
/// Contains regex patterns for parsing log entries.
/// </summary>
/// <remarks>
/// Provides compiled regular expressions for splitting and parsing different types of log entries.
///
/// Example log formats:
/// Simple: [2024-01-01 12:00:00] [INFO] Log message
/// Detailed: [2024-01-01 12:00:00] [ERROR] Type: System.Exception
///          Message: Error occurred
///          StackTrace:
///             --> at Method() in File.cs:line 1
/// </remarks>
public static partial class RegexAttributes
{
    /// <summary>
    /// Splits log content into individual entries based on timestamp pattern.
    /// </summary>
    /// <remarks>
    /// Pattern matches the start of each log entry by looking for timestamp in format [YYYY-MM-DD HH:mm:ss].
    /// Uses positive lookahead to preserve the timestamp in the split result.
    ///
    /// Example matches:
    /// [2024-01-01 12:00:00]
    /// [2023-12-31 23:59:59]
    /// </remarks>
    /// <returns>Compiled regex for splitting log entries</returns>
    [GeneratedRegex(@"(?=\[\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\])", RegexOptions.Singleline)]
    public static partial Regex LogEntrySplitter();

    /// <summary>
    /// Parses simple log entries with timestamp, level and message.
    /// </summary>
    /// <remarks>
    /// Pattern captures three groups:
    /// 1. Timestamp
    /// 2. Log level
    /// 3. Message content
    ///
    /// Example match:
    /// [2024-01-01 12:00:00] [INFO] User logged in
    /// </remarks>
    /// <returns>Compiled regex for parsing simple log entries</returns>
    [GeneratedRegex(@"^\[(.*?)\] \[(.*?)\] (.+)$", RegexOptions.Singleline)]
    public static partial Regex SimpleLogEntry();

    /// <summary>
    /// Parses detailed log entries containing exception information.
    /// </summary>
    /// <remarks>
    /// Pattern captures five groups:
    /// 1. Timestamp
    /// 2. Log level
    /// 3. Exception type
    /// 4. Exception message
    /// 5. Stack trace lines (each prefixed with "   --> ")
    ///
    /// Example match:
    /// [2024-01-01 12:00:00] [ERROR] Type: System.Exception
    /// Message: Operation failed
    /// StackTrace:
    ///    --> at Method() in File.cs:line 1
    ///    --> at Caller() in File.cs:line 2
    /// </remarks>
    /// <returns>Compiled regex for parsing detailed log entries</returns>
    [GeneratedRegex(
        @"\[(.*?)\] \[(.*?)\] Type: (.*?)[\r\n]+Message: (.*?)[\r\n]+StackTrace:[\r\n]+((?:   --> .*(?:\r?\n|$))*)",
        RegexOptions.Singleline
    )]
    public static partial Regex DetailedLogEntry();
}
