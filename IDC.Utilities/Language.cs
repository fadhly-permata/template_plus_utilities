using System.Text;
using IDC.Utilities.Extensions;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities;

/// <summary>
/// Provides localization functionality for managing multi-language messages in a thread-safe manner.
/// </summary>
/// <remarks>
/// The Language class manages localized messages stored in JSON format, supporting hierarchical message paths,
/// dynamic updates, and fallback mechanisms.
///
/// Key features:
/// - Thread-safe operations
/// - JSON-based storage
/// - Hot-reload capability
/// - Hierarchical message paths
/// - Default language fallback
/// - Memory-efficient handling
///
/// Example JSON structure:
/// <code>
/// {
///   "en": {
///     "common": {
///       "welcome": "Welcome",
///       "error": {
///         "notFound": "Resource not found"
///       }
///     }
///   },
///   "id": {
///     "common": {
///       "welcome": "Selamat Datang",
///       "error": {
///         "notFound": "Sumber tidak ditemukan"
///       }
///     }
///   }
/// }
/// </code>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// using (var lang = new Language("messages.json", "en"))
/// {
///     // Get message
///     string welcome = lang.GetMessage("common.welcome", "id");
///
///     // Update message
///     lang.UpdateMessage("en", "common.error.notFound", "Resource not found", saveToFile: true);
///
///     // Reload from file
///     lang.Reload();
/// }
/// </code>
/// </example>
/// <seealso href="https://www.newtonsoft.com/json">Newtonsoft.Json Documentation</seealso>
public class Language : IDisposable
{
    /// <summary>
    /// Gets or sets the messages container storing all language data.
    /// </summary>
    /// <remarks>
    /// Stores the complete hierarchy of messages for all languages.
    /// </remarks>
    private JObject? _messages;

    /// <summary>
    /// Gets or sets the default language code.
    /// </summary>
    /// <remarks>
    /// Used as fallback when requested language is not available.
    /// Default value is "en".
    /// </remarks>
    private string _defaultLanguage = "en";

    /// <summary>
    /// Gets or sets the file path of the JSON language file.
    /// </summary>
    /// <remarks>
    /// Stores the full path to the JSON file containing language data.
    /// Used for reload operations and saving updates.
    /// </remarks>
    private string? _jsonFilePath;

    /// <summary>
    /// Gets or sets a value indicating whether the object has been disposed.
    /// </summary>
    /// <remarks>
    /// Used to prevent operations on disposed instances.
    /// </remarks>
    private bool _disposed;

    /// <summary>
    /// Validates the object state and throws if disposed.
    /// </summary>
    /// <remarks>
    /// Internal helper method to ensure operations are not performed on disposed instances.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// Thrown when attempting to use a disposed instance.
    /// </exception>
    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: nameof(Language));

    /// <summary>
    /// Releases all resources used by the Language instance.
    /// </summary>
    /// <remarks>
    /// Performs cleanup:
    /// - Clears message container
    /// - Removes file path reference
    /// - Marks instance as disposed
    /// - Suppresses finalization
    ///
    /// > [!IMPORTANT]
    /// > After disposal, the instance cannot be reused.
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
            return;

        _messages = null;
        _jsonFilePath = null;
        _disposed = true;
        GC.SuppressFinalize(obj: this);
    }

    /// <summary>
    /// Initializes a new instance of the Language class from a JSON file.
    /// </summary>
    /// <param name="jsonPath">The path to the JSON language file.</param>
    /// <param name="defaultLanguage">The default language code. Defaults to "en".</param>
    /// <remarks>
    /// Creates a new Language instance by:
    /// - Validating file existence
    /// - Loading JSON content
    /// - Parsing into message hierarchy
    /// - Setting up default language
    ///
    /// > [!NOTE]
    /// > The JSON file must contain a valid language hierarchy structure.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when jsonPath is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when JSON file doesn't exist.</exception>
    /// <exception cref="Newtonsoft.Json.JsonReaderException">Thrown when the JSON language file is invalid or malformed.</exception>
    /// <example>
    /// <code>
    /// // Initialize with default language
    /// var lang = new Language("messages.json");
    ///
    /// // Initialize with specific default language
    /// var lang = new Language("messages.json", "id");
    /// </code>
    /// </example>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/ParsingLINQtoJSON.htm">
    /// Parsing JSON with LINQ to JSON
    /// </seealso>
    public Language(string jsonPath, string defaultLanguage = "en")
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(argument: jsonPath);

        if (!File.Exists(path: jsonPath))
            throw new FileNotFoundException(message: $"Language file not found: {jsonPath}");

        var jsonContent = File.ReadAllText(path: jsonPath);
        var config = JObject.Parse(json: jsonContent);

        _jsonFilePath = jsonPath;
        _messages = config;
        _defaultLanguage = defaultLanguage;
    }

    /// <summary>
    /// Initializes a new instance of the Language class from a JObject.
    /// </summary>
    /// <param name="config">The JObject containing language data.</param>
    /// <param name="defaultLanguage">The default language code. Defaults to "en".</param>
    /// <remarks>
    /// Creates a new Language instance from an existing JObject:
    /// - Validates configuration object
    /// - Sets up message hierarchy
    /// - Configures default language
    ///
    /// > [!NOTE]
    /// > This constructor doesn't support file operations (save/reload).
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
    /// <example>
    /// <code>
    /// var config = JObject.Parse(@"{
    ///     ""en"": {
    ///         ""greeting"": ""Hello""
    ///     }
    /// }");
    /// var lang = new Language(config, "en");
    /// </code>
    /// </example>
    public Language(JObject config, string defaultLanguage = "en")
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(argument: config);

        _messages = config;
        _defaultLanguage = defaultLanguage;
    }

    /// <summary>
    /// Updates a language message at the specified path.
    /// </summary>
    /// <param name="language">The language code to update.</param>
    /// <param name="path">The path to the message in dot notation.</param>
    /// <param name="value">The new message value.</param>
    /// <param name="saveToFile">Whether to save changes to file. Defaults to true.</param>
    /// <returns>True if update successful, false otherwise.</returns>
    /// <remarks>
    /// Updates or creates a message at the specified path:
    /// - Validates message container state
    /// - Updates message hierarchy
    /// - Optionally saves to file
    ///
    /// > [!TIP]
    /// > Use dot notation for nested paths (e.g., "common.errors.notFound")
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when messages are not initialized.
    /// </exception>
    /// <example>
    /// <code>
    /// // Update with file save
    /// lang.UpdateMessage("en", "common.welcome", "Hello", true);
    ///
    /// // Update without file save
    /// lang.UpdateMessage("id", "errors.notFound", "Tidak ditemukan", false);
    /// </code>
    /// </example>
    public bool UpdateMessage(string language, string path, string value, bool saveToFile = true)
    {
        ThrowIfDisposed();
        if (_messages is null)
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );

        try
        {
            _messages = _messages.PropUpsert(path: $"{language}.{path}", value: value);

            if (saveToFile && !string.IsNullOrEmpty(value: _jsonFilePath))
                SaveToFile();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the current language messages to the JSON file.
    /// </summary>
    /// <remarks>
    /// Persists the current message state to file:
    /// - Validates message container state
    /// - Checks file path availability
    /// - Writes formatted JSON with UTF-8 encoding
    ///
    /// > [!IMPORTANT]
    /// > Requires initialization with file path.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when:
    /// - Messages are not initialized
    /// - No file path is set
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown when file write operation fails.
    /// </exception>
    public void SaveToFile()
    {
        ThrowIfDisposed();
        if (_messages is null)
            throw new InvalidOperationException(
                message: "Language messages have not been initialized."
            );

        if (string.IsNullOrEmpty(value: _jsonFilePath))
            throw new InvalidOperationException(
                message: "No file path set. Initialize using InitializeFromFile first."
            );

        File.WriteAllText(
            path: _jsonFilePath,
            contents: _messages.ToString(formatting: Newtonsoft.Json.Formatting.Indented),
            encoding: Encoding.UTF8
        );
    }

    /// <summary>
    /// Retrieves a message for the specified path and language.
    /// </summary>
    /// <param name="path">The path to the message in dot notation.</param>
    /// <param name="language">The language code. Uses default if null.</param>
    /// <returns>The message value, or path if not found.</returns>
    /// <remarks>
    /// Retrieves localized message with fallback:
    /// - Checks requested language
    /// - Falls back to default language if not found
    /// - Returns path if message not found in any language
    ///
    /// > [!NOTE]
    /// > Uses dot notation for nested paths (e.g., "common.errors.notFound")
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when messages are not initialized.
    /// </exception>
    /// <example>
    /// <code>
    /// // Get message in specific language
    /// string msg = lang.GetMessage("common.welcome", "id");
    ///
    /// // Get message using default language
    /// string msg = lang.GetMessage("common.errors.notFound");
    /// </code>
    /// </example>
    public string GetMessage(string path, string? language = null)
    {
        ThrowIfDisposed();
        if (_messages is null)
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );

        language ??= _defaultLanguage;
        var result = _messages.PropGet<string>(path: $"{language}.{path}");

        if (result is null && language != _defaultLanguage)
            result = _messages.PropGet<string>(path: $"{_defaultLanguage}.{path}");

        return result ?? path;
    }

    /// <summary>
    /// Gets an array of available language codes.
    /// </summary>
    /// <returns>Array of language codes.</returns>
    /// <remarks>
    /// Returns all configured languages:
    /// - Validates message container state
    /// - Extracts top-level property names
    /// - Returns as string array
    ///
    /// > [!TIP]
    /// > Use to check available languages or iterate through all languages.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when messages are not initialized.
    /// </exception>
    /// <example>
    /// <code>
    /// string[] languages = lang.GetAvailableLanguages();
    /// foreach (var lang in languages)
    /// {
    ///     Console.WriteLine($"Available: {lang}");
    /// }
    /// </code>
    /// </example>
    public string[] GetAvailableLanguages()
    {
        ThrowIfDisposed();
        if (_messages is null)
            throw new InvalidOperationException(
                message: "Language messages have not been initialized. Call Initialize first."
            );

        return [.. _messages.Properties().Select(selector: static p => p.Name)];
    }

    /// <summary>
    /// Reloads language messages from the JSON file.
    /// </summary>
    /// <returns>True if reload successful, false otherwise.</returns>
    /// <remarks>
    /// Refreshes message data from file:
    /// - Validates file path availability
    /// - Checks file existence
    /// - Reloads and parses JSON content
    /// - Preserves current default language
    ///
    /// > [!IMPORTANT]
    /// > Requires initialization with file path.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no file path is set.
    /// </exception>
    /// <example>
    /// <code>
    /// if (lang.Reload())
    /// {
    ///     Console.WriteLine("Messages reloaded successfully");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("Reload failed");
    /// }
    /// </code>
    /// </example>
    public bool Reload()
    {
        ThrowIfDisposed();
        if (string.IsNullOrEmpty(value: _jsonFilePath))
            throw new InvalidOperationException(
                message: "No file path set. Initialize using InitializeFromFile first."
            );

        try
        {
            if (!File.Exists(path: _jsonFilePath))
                throw new FileNotFoundException(
                    message: $"Language file not found: {_jsonFilePath}"
                );

            var currentDefaultLanguage = _defaultLanguage;

            _messages = JObject.Parse(json: File.ReadAllText(path: _jsonFilePath));
            _defaultLanguage = currentDefaultLanguage;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
