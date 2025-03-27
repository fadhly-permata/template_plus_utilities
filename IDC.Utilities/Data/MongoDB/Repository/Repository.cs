using MongoDB.Bson;
using MongoDB.Driver;

namespace IDC.Utilities.Data.MongoDB;

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

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed, nameof(Repository<T>));

    public void Dispose()
    {
        if (_disposed)
            return;

        _collection = null;
        _collectionInitialized = false;
        _disposed = true;
        GC.SuppressFinalize(obj: this);
    }

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
}
