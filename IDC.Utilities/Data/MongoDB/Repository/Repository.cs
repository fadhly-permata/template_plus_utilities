using MongoDB.Bson;
using MongoDB.Driver;

namespace IDC.Utilities.Data.MongoDB;

/// <summary>
/// Generic repository class for MongoDB operations with built-in disposal management.
/// </summary>
/// <typeparam name="T">The type of documents stored in the collection.</typeparam>
/// <remarks>
/// Provides a high-level abstraction for MongoDB CRUD operations using <see cref="MongoHelper"/>.
/// Implements both <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/> for proper resource management.
///
/// Features:
/// - Automatic collection initialization and management
/// - Thread-safe operations
/// - Fluent interface design
/// - Built-in disposal pattern
///
/// Example:
/// <code>
/// // Synchronous usage
/// using var repo = new Repository&lt;User&gt;(
///     mongoHelper: mongoHelper,
///     collectionName: "users"
/// );
///
/// // Asynchronous usage
/// await using var repo = new Repository&lt;User&gt;(
///     mongoHelper: mongoHelper,
///     collectionName: "users"
/// );
///
/// // Method chaining
/// await repo.InitCollection()
///          .InsertOneAsync(document: user);
/// </code>
/// </remarks>
/// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/"/>
/// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/repositories/"/>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="mongoHelper"/> is null.</exception>
public sealed partial class Repository<T>(
    MongoHelper mongoHelper,
    string collectionName,
    IMongoCollection<BsonDocument>? collection = null
) : IDisposable, IAsyncDisposable
{
    private readonly MongoHelper _mongoHelper =
        mongoHelper ?? throw new ArgumentNullException(paramName: nameof(mongoHelper));
    private readonly string _collectionName = collectionName;
    private IMongoCollection<BsonDocument>? _collection = collection;

    private bool _collectionInitialized = false;
    private bool _disposed;

    /// <summary>
    /// Throws if the repository has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed, nameof(Repository<T>));

    /// <summary>
    /// Releases all resources used by the repository.
    /// </summary>
    /// <remarks>
    /// Performs cleanup by:
    /// - Clearing collection reference
    /// - Setting disposed flag
    /// - Suppressing finalization
    ///
    /// Example:
    /// <code>
    /// using (var repo = new Repository&lt;User&gt;(mongoHelper, "users"))
    /// {
    ///     // Repository operations
    /// } // Automatically disposed
    /// </code>
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
            return;

        _collection = null;
        _collectionInitialized = false;
        _disposed = true;
        GC.SuppressFinalize(obj: this);
    }

    /// <summary>
    /// Asynchronously releases all resources used by the repository.
    /// </summary>
    /// <remarks>
    /// Performs cleanup by:
    /// - Clearing collection reference
    /// - Setting disposed flag
    /// - Suppressing finalization
    ///
    /// Example:
    /// <code>
    /// await using (var repo = new Repository&lt;User&gt;(mongoHelper, "users"))
    /// {
    ///     // Repository operations
    /// } // Automatically disposed
    /// </code>
    /// </remarks>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _collection = null;
        _collectionInitialized = false;
        _disposed = true;
        GC.SuppressFinalize(obj: this);
        await ValueTask.CompletedTask;
    }

    /// <summary>
    /// Initializes the MongoDB collection for this repository.
    /// </summary>
    /// <param name="collectionName">Optional collection name. If null, uses the default collection name.</param>
    /// <returns>The current repository instance for method chaining.</returns>
    /// <remarks>
    /// This method initializes or reinitializes the MongoDB collection used by this repository.
    /// If no collection name is provided, it uses the collection name specified during repository construction.
    ///
    /// Example:
    /// <code>
    /// repository.InitCollection("newCollection")
    ///          .InsertOne(document);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when both collectionName parameter and default collection name are null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the repository has been disposed.</exception>
    public Repository<T> InitCollection(string? collectionName = null)
    {
        ThrowIfDisposed();

        if (collectionName is null && _collectionName is null)
            throw new ArgumentNullException(paramName: nameof(collectionName));

        if (_collectionInitialized)
            return this;

        _collection = _mongoHelper.GetCollection<BsonDocument>(
            collectionName: collectionName
                ?? (
                    !string.IsNullOrWhiteSpace(_collectionName)
                        ? _collectionName
                        : throw new ArgumentNullException(paramName: nameof(collectionName))
                )
        );

        _collectionInitialized = true;

        return this;
    }

    private IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
        ThrowIfDisposed();

        if (_collectionInitialized)
            return _collection!;

        _collection = _mongoHelper.GetCollection<BsonDocument>(collectionName);
        _collectionInitialized = true;

        return _collection;
    }
}
