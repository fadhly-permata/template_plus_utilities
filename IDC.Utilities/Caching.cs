using System.Collections.Concurrent;

namespace IDC.Utilities;

/// <summary>
/// Provides a thread-safe in-memory caching mechanism with expiration support.
/// </summary>
/// <remarks>
/// This class implements a simple caching system using a <see cref="ConcurrentDictionary{TKey,TValue}"/>.
/// It supports operations like setting, getting, updating, and removing cached items.
/// Each cached item has an expiration time, and a cleanup timer removes expired items periodically.
///
/// Example usage:
/// <code>
/// var cache = new Caching();
/// cache.Set("key1", "value1");
/// var value = cache.Get&lt;string&gt;("key1");
/// </code>
/// </remarks>
public class Caching : IDisposable
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = [];
    private readonly Timer _cleanupTimer;
    private readonly int _defaultExpirationMinutes;
    private bool _disposed;

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the object has been disposed.
    /// </summary>
    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: nameof(Caching));

    /// <summary>
    /// Disposes of the Caching instance, releasing all resources.
    /// </summary>
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
    /// Represents an item in the cache with its value and expiration time.
    /// </summary>
    private class CacheItem
    {
        /// <summary>
        /// Gets or sets the cached value.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the cached item.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Caching"/> class.
    /// </summary>
    /// <param name="defaultExpirationMinutes">The default expiration time in minutes for cached items.</param>
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
    /// Sets a value in the cache with the specified key and optional expiration time.
    /// </summary>
    /// <param name="key">The key to associate with the cached item.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expirationMinutes">Optional. The expiration time in minutes for this specific item.</param>
    /// <param name="expirationRenewal">If true and key exists, updates expiration time of existing item.</param>
    /// <returns>True if the item was successfully added or renewed in the cache; otherwise, false.</returns>
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
    /// Retrieves a value from the cache by its key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="expirationRenewal">If true, resets the item's expiration time when retrieved.</param>
    /// <param name="expirationMinutes">Optional. The new expiration time in minutes if increasing expiration.</param>
    /// <returns>The cached value if found and not expired; otherwise, default(T).</returns>
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
    /// Retrieves a value from the cache by its key, or returns a default value if not found.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key is not found in the cache.</param>
    /// <param name="saveIfNotFound">If true, saves the default value to the cache when the key is not found.</param>
    /// <param name="expirationMinutes">Optional. The expiration time in minutes for the item if saved.</param>
    /// <returns>The cached value if found and not expired; otherwise, the default value.</returns>
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
    /// Updates an existing item in the cache.
    /// </summary>
    /// <param name="key">The key of the item to update.</param>
    /// <param name="value">The new value to set.</param>
    /// <param name="expirationMinutes">Optional. The new expiration time in minutes for the updated item.</param>
    /// <returns>True if the item was successfully updated; otherwise, false.</returns>
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
    /// Removes an item from the cache by its key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(string key)
    {
        ThrowIfDisposed();
        return _cache.TryRemove(key: key, value: out _);
    }

    /// <summary>
    /// Checks if an item exists in the cache and is not expired.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <param name="expirationRenewal">If true, resets the item's expiration time when checked.</param>
    /// <param name="expirationMinutes">Optional. The new expiration time in minutes if increasing expiration.</param>
    /// <returns>True if the item exists and is not expired; otherwise, false.</returns>
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
    /// This method is called periodically by the cleanup timer to remove expired items from the cache.
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
    /// Adds a new item to the cache or updates an existing one.
    /// </summary>
    /// <param name="key">The key to associate with the cached item.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expirationMinutes">Optional. The expiration time in minutes for this specific item.</param>
    /// <returns>Always returns true.</returns>
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
    /// Retrieves all non-expired items from the cache.
    /// </summary>
    /// <param name="expirationRenewal">If true, resets the expiration time for all retrieved items.</param>
    /// <param name="expirationMinutes">Optional. The new expiration time in minutes if increasing expiration.</param>
    /// <returns>A dictionary containing all non-expired cached items.</returns>
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
    /// Retrieves a value from the cache or sets it if not found.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve or set.</typeparam>
    /// <param name="key">The key to associate with the cached item.</param>
    /// <param name="value">The value to cache if the key is not found.</param>
    /// <param name="expirationRenewal">If true, resets the expiration time when the item is retrieved.</param>
    /// <param name="expirationMinutes">Optional. The expiration time in minutes for this specific item.</param>
    /// <returns>The cached value if found, or the provided value if not found.</returns>
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
