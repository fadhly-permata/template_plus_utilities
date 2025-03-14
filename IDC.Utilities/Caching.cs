using System.Collections.Concurrent;

namespace IDC.Utilities;

/// <summary>
/// Provides thread-safe in-memory caching functionality with expiration support.
/// </summary>
/// <remarks>
/// Implements a simple memory cache with CRUD operations and automatic cleanup of expired items.
/// </remarks>
/// <example>
/// <code>
/// var cache = new MemoryCaching();
/// cache.Set(key: "user-123", value: userObject, expirationMinutes: 30);
/// var user = cache.Get&lt;UserModel&gt;(key: "user-123");
/// </code>
/// </example>
public class Caching : IDisposable
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = [];
    private readonly Timer _cleanupTimer;
    private bool _disposed;

    /// <summary>
    /// Throws ObjectDisposedException if the cache has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(Caching));
    }

    /// <summary>
    /// Disposes the cache and cleanup timer.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _cleanupTimer.Dispose();
            _cache.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private class CacheItem
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of MemoryCaching with automatic cleanup.
    /// </summary>
    public Caching()
    {
        _cleanupTimer = new Timer(
            callback: _ => Cleanup(),
            state: null,
            dueTime: TimeSpan.FromMinutes(value: 1),
            period: TimeSpan.FromMinutes(value: 1)
        );
    }

    /// <summary>
    /// Sets a value in the cache with optional expiration.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes.</param>
    /// <returns>True if set successfully, false otherwise.</returns>
    public bool Set(string key, object? value, int expirationMinutes = 30)
    {
        ThrowIfDisposed();
        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.AddMinutes(value: expirationMinutes)
        };
        return _cache.TryAdd(key: key, value: item);
    }

    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value or default if not found or expired.</returns>
    public T? Get<T>(string key)
    {
        ThrowIfDisposed();
        if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt > DateTime.UtcNow)
            return (T?)item.Value;
        return default;
    }

    /// <summary>
    /// Gets a value from the cache with a default value and optional save if not found.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="defaultValue">The default value to return and optionally save if the key doesn't exist.</param>
    /// <param name="saveIfNotFound">When true, saves the default value to cache if key not found.</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes when saving default value.</param>
    /// <returns>The cached value or default value if not found.</returns>
    /// <example>
    /// <code>
    /// var value = cache.Get(key: "settings", defaultValue: new Settings(), saveIfNotFound: true);
    /// </code>
    /// </example>
    public T Get<T>(
        string key,
        T defaultValue,
        bool saveIfNotFound = false,
        int expirationMinutes = 30
    )
    {
        ThrowIfDisposed();
        if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt > DateTime.UtcNow)
            return item.Value is T value ? value : defaultValue;

        if (saveIfNotFound)
            Set(key: key, value: defaultValue, expirationMinutes: expirationMinutes);

        return defaultValue;
    }

    /// <summary>
    /// Updates a value in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The new value.</param>
    /// <param name="expirationMinutes">Optional new expiration time in minutes.</param>
    /// <returns>True if updated successfully, false if key not found.</returns>
    public bool Update(string key, object? value, int expirationMinutes = 30)
    {
        ThrowIfDisposed();
        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.AddMinutes(value: expirationMinutes)
        };
        if (!_cache.TryGetValue(key: key, value: out var existingItem))
            return false;

        return _cache.TryUpdate(key: key, newValue: item, comparisonValue: existingItem);
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <returns>True if removed successfully, false if key not found.</returns>
    public bool Remove(string key)
    {
        ThrowIfDisposed();
        return _cache.TryRemove(key: key, value: out _);
    }

    /// <summary>
    /// Checks if a key exists in the cache and is not expired.
    /// </summary>
    /// <param name="key">The cache key to check.</param>
    /// <returns>True if the key exists and is not expired, false otherwise.</returns>
    public bool Exists(string key)
    {
        ThrowIfDisposed();
        return _cache.TryGetValue(key: key, value: out var item)
            && item.ExpiresAt > DateTime.UtcNow;
    }

    /// <summary>
    /// Removes all expired items from the cache.
    /// </summary>
    private void Cleanup()
    {
        var now = DateTime.UtcNow;
        foreach (var key in _cache.Keys)
        {
            if (_cache.TryGetValue(key: key, value: out var item) && item.ExpiresAt <= now)
                _cache.TryRemove(key: key, value: out _);
        }
    }

    /// <summary>
    /// Updates an existing value or inserts a new value in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to upsert.</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes.</param>
    /// <returns>True if the operation was successful.</returns>
    /// <remarks>
    /// This method will either update an existing key or add a new key if it doesn't exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// cache.Upsert(key: "user-123", value: userObject, expirationMinutes: 30);
    /// </code>
    /// </example>
    public bool Upsert(string key, object? value, int expirationMinutes = 30)
    {
        ThrowIfDisposed();
        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = DateTime.UtcNow.AddMinutes(value: expirationMinutes)
        };
        _cache.AddOrUpdate(key: key, addValue: item, updateValueFactory: (_, _) => item);
        return true;
    }

    /// <summary>
    /// Retrieves all non-expired key-value pairs from the cache.
    /// </summary>
    /// <returns>Dictionary containing all valid cache entries where key is the cache key and value is the cached value.</returns>
    /// <remarks>
    /// Only returns items that have not expired based on their ExpiresAt timestamp.
    /// The returned dictionary contains the raw cached values without their expiration metadata.
    /// </remarks>
    /// <example>
    /// <code>
    /// var allCachedItems = cache.GetAll();
    /// foreach(var (key, value) in allCachedItems) {
    ///     Console.WriteLine($"{key}: {value}");
    /// }
    /// </code>
    /// </example>
    /// <exception cref="ObjectDisposedException">Thrown when the cache has been disposed.</exception>
    public Dictionary<string, object?> GetAll()
    {
        ThrowIfDisposed();
        var now = DateTime.UtcNow;
        return _cache
            .Where(x => x.Value.ExpiresAt > now)
            .ToDictionary(x => x.Key, x => x.Value.Value);
    }
}
