namespace IDC.Template.Utilities.Helpers;

/// <summary>
/// Provides utility methods for system logging operations.
/// </summary>
public static class SystemLoggingLogic
{
    /// <summary>
    /// Gets the full path for log directory.
    /// </summary>
    /// <param name="baseDirectory">Base directory path.</param>
    /// <param name="logDirectory">Log directory name.</param>
    /// <returns>Full path to log directory.</returns>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.io.path.getfullpath">Path.GetFullPath</see>
    public static string GetFullLogPath(string baseDirectory, string logDirectory) =>
        Path.GetFullPath(
            path: Path.Combine(path1: baseDirectory, path2: logDirectory.TrimStart('/', '\\'))
        );

    /// <summary>
    /// Retrieves log files within specified date range.
    /// </summary>
    /// <param name="fullPath">Full path to log directory.</param>
    /// <param name="startTime">Start date for filtering.</param>
    /// <param name="endTime">End date for filtering.</param>
    /// <returns>Array of FileInfo for matching log files.</returns>
    /// <remarks>
    /// Searches for files matching pattern "logs-*.txt" and filters by LastWriteTime.
    /// Example:
    /// ```csharp
    /// var files = GetLogFilesByDateRange(
    ///     fullPath: "C:/logs",
    ///     startTime: DateTime.Today.AddDays(-7),
    ///     endTime: DateTime.Today
    /// );
    /// ```
    /// </remarks>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo">FileInfo Class</see>
    public static FileInfo[] GetLogFilesByDateRange(
        string fullPath,
        DateTime startTime,
        DateTime endTime
    ) =>
        [
            .. Directory
                .GetFiles(path: fullPath, searchPattern: "logs-*.txt")
                .Select(selector: f => new FileInfo(fileName: f))
                .Where(predicate: f =>
                    f.LastWriteTime.Date >= startTime.Date && f.LastWriteTime.Date <= endTime.Date
                )
                .OrderBy(keySelector: f => f.LastWriteTime)
        ];

    /// <summary>
    /// Formats file size to human-readable string.
    /// </summary>
    /// <param name="length">File size in bytes.</param>
    /// <returns>Formatted string with appropriate unit (B, KB, MB, GB, TB).</returns>
    /// <remarks>
    /// Example outputs:
    /// - 500 B
    /// - 1.50 KB
    /// - 2.25 MB
    /// - 3.00 GB
    /// - 4.50 TB
    /// </remarks>
    public static string FormatFileSize(long length) =>
        length switch
        {
            < 1024 => $"{length} B",
            < 1024 * 1024 => $"{length / 1024.0:N2} KB",
            < 1024 * 1024 * 1024 => $"{length / (1024.0 * 1024):N2} MB",
            < 1024L * 1024 * 1024 * 1024 => $"{length / (1024.0 * 1024 * 1024):N2} GB",
            _ => $"{length / (1024.0 * 1024 * 1024 * 1024):N2} TB"
        };

    /// <summary>
    /// Creates file information object with URL.
    /// </summary>
    /// <param name="file">FileInfo object.</param>
    /// <param name="requestScheme">HTTP scheme (http/https).</param>
    /// <param name="requestHost">Host name with port.</param>
    /// <returns>Anonymous object with file details and download URL.</returns>
    /// <remarks>
    /// Example response:
    /// ```json
    /// {
    ///   "Name": "logs-2024-01-01.txt",
    ///   "Size": "1.5 MB",
    ///   "Created": "2024-01-01T00:00:00",
    ///   "Modified": "2024-01-01T23:59:59",
    ///   "URL": "https://example.com/logs/logs-2024-01-01.txt"
    /// }
    /// ```
    /// </remarks>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo">FileInfo Class</see>
    public static object CreateFileInfo(FileInfo file, string requestScheme, string requestHost) =>
        new
        {
            file.Name,
            Size = FormatFileSize(file.Length),
            Created = file.CreationTime,
            Modified = file.LastWriteTime,
            URL = $"{requestScheme}://{requestHost}/logs/{file.Name}"
        };

    /// <summary>
    /// Extracts log entries from files within date range.
    /// </summary>
    /// <param name="logFiles">Array of log files.</param>
    /// <param name="startTime">Start time for filtering.</param>
    /// <param name="endTime">End time for filtering.</param>
    /// <returns>List of parsed log entries.</returns>
    /// <remarks>
    /// Reads and parses log entries from multiple files, filtering by timestamp.
    /// Example response:
    /// ```json
    /// [
    ///   {
    ///     "Timestamp": "2024-01-01T12:00:00",
    ///     "Level": "INFO",
    ///     "Type": "System",
    ///     "Message": "Application started"
    ///   }
    /// ]
    /// ```
    /// </remarks>
    public static List<object> GetLogEntries(
        FileInfo[] logFiles,
        DateTime startTime,
        DateTime endTime
    )
    {
        var logEntries = new List<object>();

        foreach (var file in logFiles)
        {
            foreach (
                var entry in RegexAttributes
                    .LogEntrySplitter()
                    .Split(input: File.ReadAllText(path: file.FullName))
                    .Where(predicate: static e => !string.IsNullOrWhiteSpace(value: e))
            )
                if (
                    TryParseLogEntry(line: entry, entry: out var parsedEntry)
                    && parsedEntry.Timestamp >= startTime
                    && parsedEntry.Timestamp <= endTime
                )
                    logEntries.Add(item: parsedEntry);
        }

        return logEntries;
    }

    /// <summary>
    /// Groups log entries by date and hour.
    /// </summary>
    /// <param name="logEntries">List of log entries.</param>
    /// <returns>Grouped log entries by date and hour.</returns>
    /// <remarks>
    /// Example response:
    /// ```json
    /// [
    ///   {
    ///     "Date": "2024-01-01",
    ///     "Total": 100,
    ///     "Hours": [
    ///       {
    ///         "Hour": 23,
    ///         "Entries": [...]
    ///       }
    ///     ]
    ///   }
    /// ]
    /// ```
    /// </remarks>
    public static object GroupLogEntries(List<object> logEntries) =>
        logEntries
            .GroupBy(keySelector: static e => ((dynamic)e).Timestamp.Date)
            .OrderByDescending(keySelector: static g => g.Key)
            .Select(static dateGroup => new
            {
                Date = dateGroup.Key.ToString("yyyy-MM-dd"),
                Total = dateGroup.Count(),
                Hours = dateGroup
                    .GroupBy(keySelector: static e => ((dynamic)e).Timestamp.Hour)
                    .OrderByDescending(keySelector: static h => h.Key)
                    .Select(selector: static hourGroup => new
                    {
                        Hour = hourGroup.Key,
                        Entries = hourGroup
                            .OrderByDescending(keySelector: static e => ((dynamic)e).Timestamp)
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

    /// <summary>
    /// Attempts to parse a log entry line.
    /// </summary>
    /// <param name="line">Log entry line to parse.</param>
    /// <param name="entry">Output parsed entry.</param>
    /// <returns>True if parsing successful, false otherwise.</returns>
    /// <remarks>
    /// Supports two log formats:
    /// 1. Simple: [Timestamp] [Level] Message
    /// 2. Detailed: [Timestamp] [Level] Type: Message --> StackTrace
    ///
    /// Example entries:
    /// ```
    /// [2024-01-01 12:00:00] [INFO] Application started
    /// [2024-01-01 12:00:00] [ERROR] Type: System.Exception: Error occurred --> Stack trace
    /// ```
    /// </remarks>
    private static bool TryParseLogEntry(string line, out dynamic entry)
    {
        entry = null!;
        try
        {
            if (!line.Contains("Type:"))
            {
                var simpleMatch = RegexAttributes.SimpleLogEntry().Match(input: line);

                if (simpleMatch.Success)
                {
                    entry = new
                    {
                        Timestamp = DateTime.Parse(simpleMatch.Groups[1].Value),
                        Level = simpleMatch.Groups[2].Value,
                        Type = string.Empty,
                        Message = simpleMatch.Groups[3].Value.Trim(),
                    };
                    return true;
                }
            }
            else
            {
                var detailedMatch = RegexAttributes.DetailedLogEntry().Match(input: line);

                if (detailedMatch.Success)
                {
                    var stackTrace = detailedMatch
                        .Groups[5]
                        .Value.Split(
                            separator: "\n   --> ",
                            options: StringSplitOptions.RemoveEmptyEntries
                        )
                        .Select(selector: static s =>
                            s.Trim().Replace(oldValue: "--> ", newValue: "")
                        )
                        .Where(predicate: static s => !string.IsNullOrWhiteSpace(value: s))
                        .ToList();

                    entry = new
                    {
                        Timestamp = DateTime.Parse(s: detailedMatch.Groups[1].Value),
                        Level = detailedMatch.Groups[2].Value,
                        Type = detailedMatch.Groups[3].Value,
                        Message = detailedMatch.Groups[4].Value,
                        StackTrace = stackTrace
                    };
                    return true;
                }
            }
        }
        catch
        {
            // Invalid log entry format
        }

        return false;
    }
}
