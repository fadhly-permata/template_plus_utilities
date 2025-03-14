using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Optional system logging instance for error and diagnostic logging.
    /// </summary>
    private readonly SystemLogging? _logging;

    /// <summary>
    /// MongoDB client instance for database connections.
    /// </summary>
    /// <see href="https://mongodb.github.io/mongo-csharp-driver/2.22/apidocs/html/T_MongoDB_Driver_MongoClient.htm">MongoClient Class</see>
    private readonly MongoClient _client;

    /// <summary>
    /// Current MongoDB database instance.
    /// </summary>
    /// <see href="https://mongodb.github.io/mongo-csharp-driver/2.22/apidocs/html/T_MongoDB_Driver_IMongoDatabase.htm">IMongoDatabase Interface</see>
    private IMongoDatabase _database;

    /// <summary>
    /// Current MongoDB client session handle for transactions.
    /// </summary>
    /// <see href="https://mongodb.github.io/mongo-csharp-driver/2.22/apidocs/html/T_MongoDB_Driver_IClientSessionHandle.htm">IClientSessionHandle Interface</see>
    private IClientSessionHandle? _session;

    /// <summary>
    /// Messages instance containing localized strings and error messages.
    /// </summary>
    private readonly Messages _messages;

    /// <summary>
    /// Flag indicating whether this instance has been disposed.
    /// </summary>
    private bool _disposed;
}
