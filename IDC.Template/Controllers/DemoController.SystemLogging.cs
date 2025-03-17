using System.Data;
using IDC.Template.Utilities;
using IDC.Template.Utilities.Helpers;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

public partial class DemoController
{
    /// <summary>
    /// Write logs an information message
    /// </summary>
    /// <param name="message">Message to log</param>
    [Tags(tags: "System Logging"), HttpPost(template: "Log/Info")]
    public APIResponse LogInfo([FromBody] string message)
    {
        try
        {
            throw new DataException(s: message);
        }
        catch (Exception ex)
        {
            return new APIResponse()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Write logs a warning message
    /// </summary>
    /// <param name="message">Message to log</param>
    [Tags(tags: "System Logging"), HttpPost(template: "Log/Warning")]
    public APIResponse LogWarning([FromBody] string message)
    {
        try
        {
            throw new DataException(s: message);
        }
        catch (Exception ex)
        {
            return new APIResponse()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Write logs an error message
    /// </summary>
    /// <param name="message">Message to log</param>
    [Tags(tags: "System Logging"), HttpPost(template: "Log/Error")]
    public APIResponse LogError([FromBody] string message)
    {
        try
        {
            throw new DataException(s: message);
        }
        catch (Exception ex)
        {
            return new APIResponse()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Write logs an error with exception details
    /// </summary>
    /// <param name="message">Message to log</param>
    [Tags(tags: "System Logging"), HttpPost(template: "Log/ErrorWithException")]
    public APIResponse LogErrorWithException([FromBody] string message)
    {
        try
        {
            throw new DataException(s: message);
        }
        catch (Exception ex)
        {
            return new APIResponse()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets list of log files from configured directory
    /// </summary>
    /// <returns>List of log files with their details</returns>
    [Tags(tags: "System Logging"), HttpGet(template: "Log/Files")]
    public APIResponseData<List<object>> GetLogFiles()
    {
        try
        {
            var fullPath = SystemLoggingLogic.GetFullLogPath(
                baseDirectory: appConfigs.Get(
                    path: "Logging.baseDirectory",
                    defaultValue: Directory.GetCurrentDirectory()
                ),
                logDirectory: appConfigs.Get(path: "Logging.LogDirectory", defaultValue: "logs")
            );

            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException(
                    message: string.Format(
                        language.GetMessage(path: "logging.directory_not_found"),
                        fullPath
                    )
                );

            var files = Directory
                .GetFiles(fullPath, "logs-*.txt")
                .Select(f => new FileInfo(f))
                .Select(f =>
                    SystemLoggingLogic.CreateFileInfo(
                        file: f,
                        requestScheme: Request.Scheme,
                        requestHost: Request.Host.Value
                    )
                )
                .OrderByDescending(f => ((dynamic)f).Modified)
                .ToList<object>();

            return new APIResponseData<List<object>>()
                .ChangeStatus(language: language, key: "api.status.success")
                .ChangeData(data: files);
        }
        catch (Exception ex)
        {
            return new APIResponseData<List<object>>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets log entries between specified time range
    /// </summary>
    /// <param name="startTime">Start time in ISO 8601 format</param>
    /// <param name="endTime">End time in ISO 8601 format</param>
    /// <returns>List of log entries within the specified time range</returns>
    [Tags(tags: "System Logging"), HttpGet(template: "Log/Read")]
    public APIResponseData<List<object>> ReadLogs(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime
    )
    {
        try
        {
            var fullPath = SystemLoggingLogic.GetFullLogPath(
                baseDirectory: appConfigs.Get(
                    path: "Logging.baseDirectory",
                    defaultValue: Directory.GetCurrentDirectory()
                ),
                logDirectory: appConfigs.Get(path: "Logging.LogDirectory", defaultValue: "logs")
            );

            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException(
                    message: string.Format(
                        language.GetMessage(path: "logging.directory_not_found"),
                        fullPath
                    )
                );

            var logFiles = SystemLoggingLogic.GetLogFilesByDateRange(
                fullPath: fullPath,
                startTime: startTime,
                endTime: endTime
            );

            var logEntries = SystemLoggingLogic.GetLogEntries(
                logFiles: logFiles,
                startTime: startTime,
                endTime: endTime
            );

            var groupedEntries = SystemLoggingLogic.GroupLogEntries(logEntries: logEntries);

            return new APIResponseData<List<object>>()
                .ChangeStatus(language: language, key: "api.status.success")
                .ChangeData(data: [groupedEntries]);
        }
        catch (Exception ex)
        {
            return new APIResponseData<List<object>>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
