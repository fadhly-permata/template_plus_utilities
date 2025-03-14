using System.Text;
using IDC.Utilities.Extensions;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities;

/// <summary>
/// Handles language localization using JSON files with thread-safe operations.
/// </summary>
/// <remarks>
/// Provides functionality to load, update, and manage language messages stored in JSON format.
/// The JSON structure should follow the pattern: { "languageCode": { "message.path": "value" } }.
///
/// Example JSON structure:
/// {
///   "en": {
///     "greeting": "Hello",
///     "errors.notfound": "Not found"
///   },
///   "id": {
///     "greeting": "Halo",
///     "errors.notfound": "Tidak ditemukan"
///   }
/// }
/// </remarks>
/// <example>
/// <code>
/// using var lang = new Language(jsonPath: "messages.json", defaultLanguage: "en");
/// var message = lang.GetMessage(path: "greeting", language: "id"); // Returns "Halo"
/// lang.UpdateMessage(language: "en", path: "new.key", value: "New Value");
/// </code>
/// </example>
public class Language : IDisposable
{
    private JObject? _messages;
    private string _defaultLanguage = "en";
    private string? _jsonFilePath;
    private bool _disposed;

    /// <summary>
    /// Throws ObjectDisposedException if the instance has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is accessed after disposal.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(Language));
    }

    /// <summary>
    /// Releases resources used by the Language instance.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _messages = null;
            _jsonFilePath = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Initializes a new instance of Language class from a JSON file.
    /// </summary>
    /// <param name="jsonPath">Path to the JSON language file.</param>
    /// <param name="defaultLanguage">Default language code to use when specific language is not found.</param>
    /// <exception cref="ArgumentNullException">Thrown when jsonPath is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when language file is not found.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public Language(string jsonPath, string defaultLanguage = "en")
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(argument: jsonPath);

        if (!File.Exists(path: jsonPath))
        {
            throw new FileNotFoundException(message: $"Language file not found: {jsonPath}");
        }

        var jsonContent = File.ReadAllText(path: jsonPath);
        var config = JObject.Parse(json: jsonContent);

        _jsonFilePath = jsonPath;
        _messages = config;
        _defaultLanguage = defaultLanguage;
    }

    /// <summary>
    /// Initializes a new instance of Language class from a JObject.
    /// </summary>
    /// <param name="config">JObject containing language messages.</param>
    /// <param name="defaultLanguage">Default language code to use when specific language is not found.</param>
    /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public Language(JObject config, string defaultLanguage = "en")
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(argument: config);
        _messages = config;
        _defaultLanguage = defaultLanguage;
    }

    /// <summary>
    /// Updates or adds a message for a specific language and path.
    /// </summary>
    /// <param name="language">Language code to update.</param>
    /// <param name="path">Message path in dot notation.</param>
    /// <param name="value">New message value.</param>
    /// <param name="saveToFile">Whether to save changes to file immediately.</param>
    /// <returns>True if update successful, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when messages are not initialized.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public bool UpdateMessage(string language, string path, string value, bool saveToFile = true)
    {
        ThrowIfDisposed();
        if (_messages is null)
        {
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );
        }

        try
        {
            _messages = _messages.PropUpsert(path: $"{language}.{path}", value: value);

            if (saveToFile && !string.IsNullOrEmpty(value: _jsonFilePath))
            {
                SaveToFile();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Saves current messages to the JSON file.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when messages are not initialized or file path is not set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public void SaveToFile()
    {
        ThrowIfDisposed();
        if (_messages is null)
        {
            throw new InvalidOperationException(
                message: "Language messages have not been initialized."
            );
        }

        if (string.IsNullOrEmpty(value: _jsonFilePath))
        {
            throw new InvalidOperationException(
                message: "No file path set. Initialize using InitializeFromFile first."
            );
        }

        var jsonContent = _messages.ToString(formatting: Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path: _jsonFilePath, contents: jsonContent, encoding: Encoding.UTF8);
    }

    /// <summary>
    /// Gets a message by path and language.
    /// </summary>
    /// <param name="path">Message path in dot notation.</param>
    /// <param name="language">Optional language code. Uses default if not specified.</param>
    /// <returns>Localized message or path if not found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when messages are not initialized.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public string GetMessage(string path, string? language = null)
    {
        ThrowIfDisposed();
        if (_messages is null)
        {
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );
        }

        language ??= _defaultLanguage;

        var result = _messages.PropGet<string>(path: $"{language}.{path}");

        if (result is null && language != _defaultLanguage)
        {
            result = _messages.PropGet<string>(path: $"{_defaultLanguage}.{path}");
        }

        return result ?? path;
    }

    /// <summary>
    /// Gets list of available language codes.
    /// </summary>
    /// <returns>Array of language codes.</returns>
    /// <exception cref="InvalidOperationException">Thrown when messages are not initialized.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public string[] GetAvailableLanguages()
    {
        ThrowIfDisposed();
        if (_messages is null)
        {
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );
        }

        return [.. _messages.Properties().Select(selector: static p => p.Name)];
    }

    /// <summary>
    /// Reloads messages from the JSON file.
    /// </summary>
    /// <returns>True if reload successful, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when file path is not set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
    public bool Reload()
    {
        ThrowIfDisposed();
        if (string.IsNullOrEmpty(value: _jsonFilePath))
        {
            throw new InvalidOperationException(
                message: "No file path set. Initialize using InitializeFromFile first."
            );
        }

        try
        {
            if (!File.Exists(path: _jsonFilePath))
            {
                throw new FileNotFoundException(
                    message: $"Language file not found: {_jsonFilePath}"
                );
            }

            var jsonContent = File.ReadAllText(path: _jsonFilePath);
            var config = JObject.Parse(json: jsonContent);
            var currentDefaultLanguage = _defaultLanguage;

            _messages = config;
            _defaultLanguage = currentDefaultLanguage;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
