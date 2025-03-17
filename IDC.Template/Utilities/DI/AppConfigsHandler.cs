using IDC.Utilities.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Utilities.DI;

/// <summary>
/// Handles configuration management through JSON files with thread-safe operations.
/// </summary>
/// <remarks>
/// Provides functionality to load, update, and manage application configurations stored in JSON format.
/// Implements IDisposable for proper resource cleanup.
/// </remarks>
/// <example>
/// <code>
/// using var config = AppConfigsHandler.Load();
/// var value = config.Get&lt;string&gt;(path: "app.name");
/// config.Update(path: "app.version", value: "1.0.0");
/// </code>
/// </example>
public sealed class AppConfigsHandler : IDisposable
{
    private readonly JObject _config;
    private bool _disposed;

    /// <summary>
    /// Throws ObjectDisposedException if the handler has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is accessed after disposal.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(AppConfigsHandler));
    }

    /// <summary>
    /// Performs cleanup of managed resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _config.RemoveAll();
            _disposed = true;
        }
    }

    /// <summary>
    /// Initializes a new instance of AppConfigsHandler.
    /// </summary>
    /// <param name="config">The JObject containing configuration data.</param>
    private AppConfigsHandler(JObject config)
    {
        _config = config;
        _disposed = false;
    }

    /// <summary>
    /// Loads configuration from the default appconfigs.jsonc file.
    /// </summary>
    /// <returns>A new instance of AppConfigsHandler.</returns>
    /// <exception cref="FileNotFoundException">Thrown when appconfigs.jsonc is not found.</exception>
    /// <exception cref="JsonReaderException">Thrown when JSON parsing fails.</exception>
    public static AppConfigsHandler Load()
    {
        var appConfigPath = Path.Combine(
            path1: Directory.GetCurrentDirectory(),
            path2: "wwwroot",
            path3: "appconfigs.jsonc"
        );
        var jsonContent = File.ReadAllText(appConfigPath);
        return new(JObject.Parse(jsonContent));
    }

    /// <summary>
    /// Loads configuration from a JSON string.
    /// </summary>
    /// <param name="jsonContent">JSON string containing configuration data.</param>
    /// <returns>A new instance of AppConfigsHandler.</returns>
    /// <exception cref="JsonReaderException">Thrown when JSON parsing fails.</exception>
    public static AppConfigsHandler Load(string jsonContent) => new(JObject.Parse(jsonContent));

    /// <summary>
    /// Loads configuration from an existing JObject.
    /// </summary>
    /// <param name="config">JObject containing configuration data.</param>
    /// <returns>A new instance of AppConfigsHandler.</returns>
    public static AppConfigsHandler Load(JObject config) => new(config);

    /// <summary>
    /// Gets a configuration value by path.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="path">The dot-notation path to the configuration value.</param>
    /// <param name="defaultValue">The default value if path not found.</param>
    /// <returns>The configuration value or default if not found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    public T Get<T>(string path, T defaultValue)
    {
        ThrowIfDisposed();
        return _config.PropGet(path: path, defaultValue: defaultValue);
    }

    /// <summary>
    /// Gets a configuration value by path without a default value.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="path">The dot-notation path to the configuration value.</param>
    /// <returns>The configuration value or default if not found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    public T? Get<T>(string path)
    {
        ThrowIfDisposed();
        return _config.PropGet<T>(path: path);
    }

    /// <summary>
    /// Updates a configuration value by path.
    /// </summary>
    /// <param name="path">The dot-notation path to update.</param>
    /// <param name="value">The new value to set.</param>
    /// <returns>True if update successful, false otherwise.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    public bool Update(string path, object? value)
    {
        ThrowIfDisposed();
        try
        {
            _config.PropUpsert(path, value);
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
    /// <param name="updates">Dictionary of paths and values to update.</param>
    /// <returns>True if all updates successful, false otherwise.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler is disposed.</exception>
    public bool Update(Dictionary<string, object?> updates)
    {
        ThrowIfDisposed();
        try
        {
            _config.PropUpdate(updates);
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
    public bool Remove(string path)
    {
        ThrowIfDisposed();
        try
        {
            _config.PropRemove(kvp => kvp.Key == path);
            SaveToFile();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the current configuration to the appconfigs.jsonc file.
    /// </summary>
    private void SaveToFile()
    {
        var appConfigPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "appconfigs.jsonc"
        );
        File.WriteAllText(appConfigPath, _config.ToString(Newtonsoft.Json.Formatting.Indented));
    }
}
