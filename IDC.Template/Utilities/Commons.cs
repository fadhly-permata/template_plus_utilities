namespace IDC.Template.Utilities;

/// <summary>
/// Provides common utility methods and functionality for the application.
/// </summary>
/// <remarks>
/// This static class contains helper methods that can be used across the application
/// for common operations like environment checks.
/// </remarks>
/// <example>
/// <code>
/// bool isDebug = Commons.IsDebugEnvironment();
/// </code>
/// </example>
public static class Commons
{
    /// <summary>
    /// Gets the current environment status (debug or not).
    /// </summary>
    /// <returns>True if the environment is in debug mode, false otherwise.</returns>
    /// <remarks>
    /// This method checks if the application is running in debug mode.
    /// </remarks>
    /// <example>
    /// bool isDebug = Commons.IsDebugEnvironment();
    /// </example>
    public static bool IsDebugEnvironment()
    {
        return string.Equals(
            a: Environment.GetEnvironmentVariable(variable: "ASPNETCORE_ENVIRONMENT"),
            b: "Development",
            comparisonType: StringComparison.OrdinalIgnoreCase
        );
    }
}
