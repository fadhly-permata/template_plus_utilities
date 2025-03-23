using MongoDB.Driver;

namespace IDC.Utilities.Data;

/// <summary>
/// Provides a thread-safe helper for MongoDB database operations with built-in logging and session management.
/// </summary>
/// <remarks>
/// This helper class encapsulates MongoDB client operations with additional features:
/// - Thread-safe database operations
/// - Integrated logging system
/// - Session management
/// - Connection pooling
/// - Custom error messages
///
/// Example usage:
/// <code>
/// using var mongo = new MongoHelper(connectionString: "mongodb://localhost:27017");
/// await mongo.ConnectAsync(database: "mydb");
/// var collection = mongo.GetCollection&lt;User&gt;("users");
/// </code>
/// </remarks>
public sealed partial class MongoHelper
{
    /// <summary>
    /// Provides logging capabilities for MongoDB operations and error tracking.
    /// </summary>
    /// <remarks>
    /// When configured, logs various events:
    /// - Connection attempts
    /// - Query execution
    /// - Error conditions
    /// - Performance metrics
    ///
    /// > [!NOTE]
    /// > Logging is optional and can be disabled by passing null during initialization.
    /// </remarks>
    private readonly SystemLogging? _logging;

    /// <summary>
    /// Manages the MongoDB client connection with automatic connection pooling.
    /// </summary>
    /// <remarks>
    /// Handles:
    /// - Connection pooling
    /// - Server discovery
    /// - Connection lifecycle
    /// - Authentication
    ///
    /// > [!IMPORTANT]
    /// > This instance is thread-safe and should be reused across the application.
    /// </remarks>
    private readonly MongoClient _client;

    /// <summary>
    /// Represents the currently active MongoDB database instance.
    /// </summary>
    /// <remarks>
    /// Provides access to:
    /// - Collections
    /// - Database operations
    /// - Index management
    /// - Database statistics
    ///
    /// > [!WARNING]
    /// > Must be initialized via <see cref="M:IDC.Utilities.Data.MongoHelper.ConnectAsync"/> before use.
    /// </remarks>
    private IMongoDatabase _database;

    /// <summary>
    /// Manages the current MongoDB session for transaction support.
    /// </summary>
    /// <remarks>
    /// Enables:
    /// - Transaction management
    /// - Causal consistency
    /// - Session-based operations
    ///
    /// > [!TIP]
    /// > Sessions are optional and primarily used for transactions.
    /// </remarks>
    private IClientSessionHandle? _session;

    /// <summary>
    /// Contains customizable message templates for various operations.
    /// </summary>
    /// <remarks>
    /// Provides:
    /// - Error messages
    /// - Success notifications
    /// - Operation status updates
    ///
    /// > [!NOTE]
    /// > Messages support localization through the template system.
    /// </remarks>
    private Messages _messages = new();

    /// <summary>
    /// Indicates whether this instance has been disposed.
    /// </summary>
    /// <remarks>
    /// Used to:
    /// - Prevent use after disposal
    /// - Ensure proper resource cleanup
    /// - Track object lifecycle
    ///
    /// > [!CAUTION]
    /// > Operations on a disposed instance will throw <see cref="ObjectDisposedException"/>.
    /// </remarks>
    private bool _disposed;
}
