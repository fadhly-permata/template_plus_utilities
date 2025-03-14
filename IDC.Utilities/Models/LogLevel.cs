namespace IDC.Utilities.Models;

/// <summary>
/// Defines logging severity levels.
/// </summary>
/// <remarks>
/// Used to categorize log messages based on their severity.
/// </remarks>
public enum LogLevel
{
    /// <summary>
    /// Information level logs for general operational messages.
    /// </summary>
    Information,

    /// <summary>
    /// Warning level logs for potentially harmful situations.
    /// </summary>
    Warning,

    /// <summary>
    /// Error level logs for error conditions and exceptions.
    /// </summary>
    Error
}