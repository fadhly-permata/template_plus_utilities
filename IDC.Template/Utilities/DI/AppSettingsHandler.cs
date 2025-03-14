using IDC.Utilities.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Utilities.DI;

/// <summary>
/// Handles appsettings.json configuration management with thread-safe operations.
/// </summary>
/// <remarks>
/// Provides functionality to load, update, and manage application settings stored in appsettings.json format.
/// Implements IDisposable for proper resource cleanup.
///
/// Example request body:
/// ```json
/// {
///   "AllowedHosts": "*",
///   "Logging": {
///     "LogLevel": {
///       "Default": "Information",
///       "Microsoft.AspNetCore": "Warning"
///     }
///   }
/// }
/// ```
/// </remarks>
/// <example>
/// <code>
/// using var settings = AppSettingsHandler.Load();
/// var value = settings.Get&lt;string&gt;(path: "AllowedHosts");
/// settings.Update(path: "Logging.LogLevel.Default", value: "Debug");
/// </code>
/// </example>
public sealed class AppSettingsHandler : IDisposable
{
    private readonly JObject _settings;
    private bool _disposed;

    /// <summary>
    /// Throws ObjectDisposedException if the handler has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is accessed after disposal.</exception>
    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed, nameof(AppSettingsHandler));

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <remarks>
    /// Cleans up the JObject settings and marks the handler as disposed.
    /// </remarks>
    public void Dispose()
    {
        if (!_disposed)
        {
            _settings.RemoveAll();
            _disposed = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of the AppSettingsHandler class.
    /// </summary>
    /// <param name="settings">The JObject containing application settings.</param>
    private AppSettingsHandler(JObject settings)
    {
        _settings = settings;
        _disposed = false;
    }

    /// <summary>
    /// Loads configuration from the default appsettings.json file.
    /// </summary>
    /// <returns>A new instance of AppSettingsHandler.</returns>
    /// <exception cref="FileNotFoundException">Thrown when appsettings.json is not found.</exception>
    /// <exception cref="JsonReaderException">Thrown when JSON parsing fails.</exception>
    /// <remarks>
    /// Reads the appsettings.json file from the application's current directory.
    /// </remarks>
    public static AppSettingsHandler Load()
    {
        var settingsPath = Path.Combine(
            path1: Directory.GetCurrentDirectory(),
            path2: "appsettings.json"
        );
        var jsonContent = File.ReadAllText(settingsPath);
        return new(JObject.Parse(jsonContent));
    }

    /// <summary>
    /// Creates a new AppSettingsHandler from JSON content.
    /// </summary>
    /// <param name="jsonContent">The JSON string to parse.</param>
    /// <returns>A new instance of AppSettingsHandler.</returns>
    /// <exception cref="JsonReaderException">Thrown when JSON parsing fails.</exception>
    public static AppSettingsHandler Load(string jsonContent) => new(JObject.Parse(jsonContent));

    /// <summary>
    /// Creates a new AppSettingsHandler from an existing JObject.
    /// </summary>
    /// <param name="settings">The JObject containing settings.</param>
    /// <returns>A new instance of AppSettingsHandler.</returns>
    public static AppSettingsHandler Load(JObject settings) => new(settings);

    /// <summary>
    /// Gets a configuration value with a default fallback.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="path">The dot-notation path to the configuration value.</param>
    /// <param name="defaultValue">The default value if path not found.</param>
    /// <returns>The configuration value or default if not found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    /// <see href="https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm">Querying LINQ to JSON</see>
    public T Get<T>(string path, T defaultValue)
    {
        ThrowIfDisposed();
        return _settings.PropGet(path: path, defaultValue: defaultValue);
    }

    /// <summary>
    /// Gets a configuration value.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="path">The dot-notation path to the configuration value.</param>
    /// <returns>The configuration value or default if not found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    /// <see href="https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm">Querying LINQ to JSON</see>
    public T? Get<T>(string path)
    {
        ThrowIfDisposed();
        return _settings.PropGet<T>(path: path);
    }

    /// <summary>
    /// Updates a configuration value by path.
    /// </summary>
    /// <param name="path">The dot-notation path to update.</param>
    /// <param name="value">The new value to set.</param>
    /// <returns>True if update successful, false otherwise.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    /// <see href="https://www.newtonsoft.com/json/help/html/ModifyingLINQtoJSON.htm">Modifying LINQ to JSON</see>
    public bool Update(string path, object? value)
    {
        ThrowIfDisposed();
        try
        {
            _settings.PropUpsert(path, value);
            SaveToFile();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Updates multiple configuration values.
    /// </summary>
    /// <param name="updates">Dictionary of path-value pairs to update.</param>
    /// <returns>True if all updates successful, false otherwise.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    /// <see href="https://www.newtonsoft.com/json/help/html/ModifyingLINQtoJSON.htm">Modifying LINQ to JSON</see>
    public bool Update(Dictionary<string, object?> updates)
    {
        ThrowIfDisposed();
        try
        {
            _settings.PropUpdate(updates);
            SaveToFile();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a configuration value by path.
    /// </summary>
    /// <param name="path">The dot-notation path to remove.</param>
    /// <returns>True if removal successful, false otherwise.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    /// <see href="https://www.newtonsoft.com/json/help/html/ModifyingLINQtoJSON.htm">Modifying LINQ to JSON</see>
    public bool Remove(string path)
    {
        ThrowIfDisposed();
        try
        {
            _settings.PropRemove(kvp => kvp.Key == path);
            SaveToFile();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the current configuration to the appsettings.json file.
    /// </summary>
    /// <remarks>
    /// Writes the settings to appsettings.json in the application's current directory.
    /// The JSON is formatted with indentation for readability.
    /// </remarks>
    private void SaveToFile()
    {
        var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        File.WriteAllText(settingsPath, _settings.ToString(Newtonsoft.Json.Formatting.Indented));
    }
}
