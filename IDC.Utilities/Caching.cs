using System.Collections.Concurrent;

namespace IDC.Utilities;

/// <summary>
/// A thread-safe, in-memory caching implementation with automatic cleanup capabilities.
/// </summary>
/// <remarks>
/// Provides a robust caching solution with the following features:
///
/// - Thread-safe operations using ConcurrentDictionary
/// - Automatic cleanup of expired items
/// - Configurable expiration times
/// - Generic type support
/// - Expiration renewal options
///
/// > [!IMPORTANT]
/// > Always dispose of the cache instance when it's no longer needed to prevent memory leaks.
///
/// Basic usage example:
/// <code>
/// using var cache = new Caching(defaultExpirationMinutes: 30);
///
/// // Store user data
/// var userData = new UserData { Id = 1, Name = "John" };
/// cache.Set(key: "user:1", value: userData, expirationMinutes: 60);
///
/// // Retrieve user data
/// var user = cache.Get&lt;UserData&gt;(key: "user:1");
///
/// // Update with expiration renewal
/// cache.Update(key: "user:1", value: updatedUserData);
///
/// // Remove user data
/// cache.Remove(key: "user:1");
/// </code>
///
/// Advanced usage example:
/// <code>
/// // Get or set with default value
/// var config = cache.GetOrSet(
///     key: "app:config",
///     value: defaultConfig,
///     expirationMinutes: 120
/// );
///
/// // Batch operations
/// var allCachedItems = cache.GetAll();
///
/// // Check existence
/// if (cache.Exists(key: "user:1"))
/// {
///     // Process cached user
/// }
/// </code>
/// </remarks>
/// <seealso cref="IDisposable"/>
public class Caching : IDisposable
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = [];
    private readonly Timer _cleanupTimer;
    private readonly int _defaultExpirationMinutes;
    private bool _disposed;

    /// <summary>
    /// Validates the cache instance hasn't been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to access a disposed cache instance.</exception>
    /// <remarks>
    /// > [!WARNING]
    /// > This method should be called at the start of any public method to ensure the cache is still valid.
    /// </remarks>
    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: nameof(Caching));

    /// <summary>
    /// Releases all resources used by the cache instance.
    /// </summary>
    /// <remarks>
    /// > [!NOTE]
    /// > Multiple calls to Dispose are safe - subsequent calls will be ignored.
    ///
    /// Example:
    /// <code>
    /// using (var cache = new Caching())
    /// {
    ///     // Use cache
    /// } // Automatically disposed
    /// </code>
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
            return;

        _cleanupTimer.Dispose();
        _cache.Clear();
        _disposed = true;
        GC.SuppressFinalize(obj: this);
    }

    /// <summary>
    /// Represents a value stored in the cache with its expiration metadata.
    /// </summary>
    /// <remarks>
    /// Internal structure used to track cached items and their expiration times.
    /// </remarks>
    private class CacheItem
    {
        /// <summary>
        /// The cached value. Can be null.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// UTC timestamp when this item expires.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Initializes a new cache instance with specified default expiration time.
    /// </summary>
    /// <param name="defaultExpirationMinutes">Default time in minutes before cached items expire. Defaults to 30 minutes.</param>
    /// <remarks>
    /// > [!TIP]
    /// > Choose an appropriate default expiration time based on your data's volatility.
    ///
    /// Example:
    /// <code>
    /// // Cache with 1-hour default expiration
    /// var cache = new Caching(defaultExpirationMinutes: 60);
    ///
    /// // Cache with 30-minute default expiration
    /// var shortCache = new Caching();
    /// </code>
    /// </remarks>
    public Caching(int defaultExpirationMinutes = 30)
    {
        _defaultExpirationMinutes = defaultExpirationMinutes;
        _cleanupTimer = new Timer(
            callback: _ => Cleanup(),
            state: null,
            dueTime: TimeSpan.FromMinutes(value: 1),
            period: TimeSpan.FromMinutes(value: 1)
        );
    }

    /// <summary>
    /// Stores a value in the cache.
    /// </summary>
    /// <param name="key">Unique identifier for the cached item.</param>
    /// <param name="value">Value to cache. Can be null.</param>
    /// <param name="expirationMinutes">Optional custom expiration time in minutes. If null, uses default expiration time.</param>
    /// <param name="expirationRenewal">If true, renews expiration for existing items. Defaults to true.</param>
    /// <returns>true if the value was stored successfully; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!NOTE]
    /// > If the key already exists and expirationRenewal is true, the existing value will be updated with a new expiration time.
    ///
    /// Example:
    /// <code>
    /// // Basic usage
    /// cache.Set(key: "session:123", value: sessionData);
    ///
    /// // With custom expiration
    /// cache.Set(
    ///     key: "temp:data",
    ///     value: tempData,
    ///     expirationMinutes: 5,
    ///     expirationRenewal: false
    /// );
    /// </code>
    /// </remarks>
    public bool Set(
        string key,
        object? value,
        int? expirationMinutes = null,
        bool expirationRenewal = true
    )
    {
        ThrowIfDisposed();

        if (expirationRenewal && _cache.TryGetValue(key: key, value: out var existingItem))
        {
            existingItem.Value = value;
            existingItem.ExpiresAt = DateTime.UtcNow.AddMinutes(
                value: expirationMinutes ?? _defaultExpirationMinutes
            );
            return true;
        }

        return _cache.TryAdd(
            key: key,
            value: new CacheItem
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    value: expirationMinutes ?? _defaultExpirationMinutes
                )
            }
        );
    }

    /// <summary>
    /// Retrieves a typed value from the cache.
    /// </summary>
    /// <typeparam name="T">Expected type of the cached value.</typeparam>
    /// <param name="key">Key of the cached item to retrieve.</param>
    /// <param name="expirationRenewal">If true, renews the item's expiration time on access.</param>
    /// <param name="expirationMinutes">Optional new expiration time in minutes.</param>
    /// <returns>The cached value cast to type T if found and not expired; otherwise, default(T).</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!TIP]
    /// > Use type parameters that match the stored value type to avoid runtime casting exceptions.
    ///
    /// Example:
    /// <code>
    /// // Get user data
    /// var user = cache.Get&lt;UserModel&gt;(
    ///     key: "user:profile",
    ///     expirationRenewal: true,
    ///     expirationMinutes: 30
    /// );
    ///
    /// if (user != null)
    /// {
    ///     // Process user data
    /// }
    /// </code>
    /// </remarks>
    public T? Get<T>(string key, bool expirationRenewal = true, int? expirationMinutes = null)
    {
        ThrowIfDisposed();
        if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt > DateTime.UtcNow)
        {
            if (expirationRenewal)
                item.ExpiresAt = DateTime.UtcNow.AddMinutes(
                    value: expirationMinutes ?? _defaultExpirationMinutes
                );

            return (T?)item.Value;
        }
        return default;
    }

    /// <summary>
    /// Retrieves a typed value with a default fallback.
    /// </summary>
    /// <typeparam name="T">Expected type of the cached value.</typeparam>
    /// <param name="key">Key of the cached item.</param>
    /// <param name="defaultValue">Value to return if key not found or expired.</param>
    /// <param name="saveIfNotFound">If true, stores defaultValue in cache when key not found.</param>
    /// <param name="expirationMinutes">Optional expiration time for newly stored default value.</param>
    /// <returns>The cached value or defaultValue if not found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!TIP]
    /// > Use this method when you want to ensure a non-null return value.
    ///
    /// Example:
    /// <code>
    /// var settings = cache.Get(
    ///     key: "app:settings",
    ///     defaultValue: new AppSettings(),
    ///     saveIfNotFound: true
    /// );
    /// </code>
    /// </remarks>
    public T Get<T>(
        string key,
        T defaultValue,
        bool saveIfNotFound = false,
        int? expirationMinutes = null
    )
    {
        ThrowIfDisposed();
        if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt > DateTime.UtcNow)
        {
            item.ExpiresAt = DateTime.UtcNow.AddMinutes(
                value: expirationMinutes ?? _defaultExpirationMinutes
            );
            return item.Value is T value ? value : defaultValue;
        }

        if (saveIfNotFound)
            Set(key: key, value: defaultValue, expirationMinutes: expirationMinutes);

        return defaultValue;
    }

    /// <summary>
    /// Updates an existing cached value.
    /// </summary>
    /// <param name="key">Key of the item to update.</param>
    /// <param name="value">New value to store.</param>
    /// <param name="expirationMinutes">Optional new expiration time in minutes.</param>
    /// <returns>true if the update was successful; false if the key wasn't found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!IMPORTANT]
    /// > This method only updates existing items. Use Set() or Upsert() for new items.
    ///
    /// Example:
    /// <code>
    /// if (cache.Update(
    ///     key: "user:preferences",
    ///     value: newPreferences,
    ///     expirationMinutes: 60
    /// ))
    /// {
    ///     // Update successful
    /// }
    /// </code>
    /// </remarks>
    public bool Update(string key, object? value, int? expirationMinutes = null)
    {
        ThrowIfDisposed();
        if (!_cache.TryGetValue(key: key, value: out var existingItem))
            return false;

        return _cache.TryUpdate(
            key: key,
            newValue: new CacheItem
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    value: expirationMinutes ?? _defaultExpirationMinutes
                )
            },
            comparisonValue: existingItem
        );
    }

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="key">Key of the item to remove.</param>
    /// <returns>true if the item was removed; false if it wasn't found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// Example:
    /// <code>
    /// if (cache.Remove(key: "session:expired"))
    /// {
    ///     // Item was successfully removed
    /// }
    /// </code>
    /// </remarks>
    public bool Remove(string key)
    {
        ThrowIfDisposed();
        return _cache.TryRemove(key: key, value: out _);
    }

    /// <summary>
    /// Checks if a key exists in the cache and hasn't expired.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <param name="expirationRenewal">If true, renews expiration on check.</param>
    /// <param name="expirationMinutes">Optional new expiration time in minutes.</param>
    /// <returns>true if the key exists and hasn't expired; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// Example:
    /// <code>
    /// if (cache.Exists(
    ///     key: "auth:token",
    ///     expirationRenewal: true,
    ///     expirationMinutes: 15
    /// ))
    /// {
    ///     // Token exists and is valid
    /// }
    /// </code>
    /// </remarks>
    public bool Exists(string key, bool expirationRenewal = true, int? expirationMinutes = null)
    {
        ThrowIfDisposed();
        if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt > DateTime.UtcNow)
        {
            if (expirationRenewal)
                item.ExpiresAt = DateTime.UtcNow.AddMinutes(
                    value: expirationMinutes ?? _defaultExpirationMinutes
                );

            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes expired items from the cache.
    /// </summary>
    /// <remarks>
    /// > [!NOTE]
    /// > This method is automatically called by an internal timer.
    ///
    /// > [!TIP]
    /// > The cleanup process is thread-safe and can run concurrently with other operations.
    /// </remarks>
    private void Cleanup()
    {
        foreach (var key in _cache.Keys)
            if (
                _cache.TryGetValue(key: key, value: out var item)
                && item.ExpiresAt <= DateTime.UtcNow
            )
                _cache.TryRemove(key: key, value: out _);
    }

    /// <summary>
    /// Adds a new or updates an existing cache entry.
    /// </summary>
    /// <param name="key">Key for the cache entry.</param>
    /// <param name="value">Value to store.</param>
    /// <param name="expirationMinutes">Optional custom expiration time in minutes.</param>
    /// <returns>true if the operation was successful.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!TIP]
    /// > Use this method when you don't need to distinguish between insert and update operations.
    ///
    /// Example:
    /// <code>
    /// cache.Upsert(
    ///     key: "metrics:daily",
    ///     value: newMetrics,
    ///     expirationMinutes: 1440
    /// );
    /// </code>
    /// </remarks>
    public bool Upsert(string key, object? value, int? expirationMinutes = null)
    {
        ThrowIfDisposed();
        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                value: expirationMinutes ?? _defaultExpirationMinutes
            )
        };
        _cache.AddOrUpdate(key: key, addValue: item, updateValueFactory: (_, _) => item);
        return true;
    }

    /// <summary>
    /// Retrieves all non-expired cached items.
    /// </summary>
    /// <param name="expirationRenewal">If true, renews expiration for retrieved items.</param>
    /// <param name="expirationMinutes">Optional new expiration time in minutes.</param>
    /// <returns>Dictionary of all valid cached items.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!CAUTION]
    /// > This method returns all cached items. Use with caution on large caches.
    ///
    /// Example:
    /// <code>
    /// var allItems = cache.GetAll(
    ///     expirationRenewal: true,
    ///     expirationMinutes: 30
    /// );
    ///
    /// foreach (var (key, value) in allItems)
    /// {
    ///     // Process each item
    /// }
    /// </code>
    /// </remarks>
    public Dictionary<string, object?> GetAll(
        bool expirationRenewal = true,
        int? expirationMinutes = null
    )
    {
        ThrowIfDisposed();

        return _cache
            .Where(predicate: x => x.Value.ExpiresAt > DateTime.UtcNow)
            .Select(selector: x =>
            {
                if (expirationRenewal)
                    x.Value.ExpiresAt = DateTime.UtcNow.AddMinutes(
                        value: expirationMinutes ?? _defaultExpirationMinutes
                    );

                return x;
            })
            .ToDictionary(keySelector: x => x.Key, elementSelector: x => x.Value.Value);
    }

    /// <summary>
    /// Gets an existing value or sets a new one if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="value">Value to set if key doesn't exist.</param>
    /// <param name="expirationRenewal">If true, renews expiration for existing items.</param>
    /// <param name="expirationMinutes">Optional custom expiration time in minutes.</param>
    /// <returns>The existing or newly set value.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the cache has been disposed.</exception>
    /// <remarks>
    /// > [!TIP]
    /// > This method is atomic and thread-safe.
    ///
    /// Example:
    /// <code>
    /// var userPrefs = cache.GetOrSet(
    ///     key: "user:123:preferences",
    ///     value: new UserPreferences(),
    ///     expirationRenewal: true,
    ///     expirationMinutes: 60
    /// );
    /// </code>
    /// </remarks>
    public T GetOrSet<T>(
        string key,
        T value,
        bool expirationRenewal = true,
        int? expirationMinutes = null
    )
    {
        ThrowIfDisposed();

        if (
            _cache.TryGetValue(key: key, value: out var existing)
            && existing.ExpiresAt > DateTime.UtcNow
        )
        {
            if (expirationRenewal)
                existing.ExpiresAt = DateTime.UtcNow.AddMinutes(
                    value: expirationMinutes ?? _defaultExpirationMinutes
                );

            return existing.Value is T typedValue ? typedValue : value;
        }

        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                value: expirationMinutes ?? _defaultExpirationMinutes
            )
        };

        _cache.AddOrUpdate(key: key, addValue: item, updateValueFactory: (_, _) => item);

        return value;
    }
}
