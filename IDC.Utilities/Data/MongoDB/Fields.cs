using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Optional logging instance for error and diagnostic messages.
    /// </summary>
    private readonly SystemLogging? _logging;

    /// <summary>
    /// The MongoDB client instance used for database operations.
    /// </summary>
    private readonly MongoClient _client;

    /// <summary>
    /// The current MongoDB database instance.
    /// </summary>
    private IMongoDatabase _database;

    /// <summary>
    /// The current MongoDB client session, if any.
    /// </summary>
    private IClientSessionHandle? _session;

    /// <summary>
    /// Customizable message strings for operations.
    /// </summary>
    private Messages _messages = new();

    /// <summary>
    /// Flag indicating if the instance has been disposed.
    /// </summary>
    private bool _disposed;
}
