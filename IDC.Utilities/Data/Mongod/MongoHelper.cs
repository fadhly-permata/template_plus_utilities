using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

/// <summary>
/// Provides helper methods for MongoDB database operations with transaction support.
/// </summary>
/// <remarks>
/// <para>
/// The MongoHelper class provides a comprehensive set of methods for interacting with MongoDB databases, including:
/// - Connection management (Connect, Disconnect, Reconnect)
/// - Transaction handling (Begin, Commit, Rollback)
/// - Query execution (ExecuteQuery, ExecuteUpsert, ExecuteUpdate, ExecuteDelete)
/// - Connection state validation
/// </para>
///
/// <para>
/// Key Features:
/// - Thread-safe database operations
/// - Automatic connection and transaction management
/// - Support for both BSON and JSON-formatted results
/// - Customizable logging and error handling
/// - Method chaining support for fluent API usage
/// </para>
///
/// <para>
/// File Structure:
/// - CTOR.cs: Constructor implementations
/// - DTOR.cs: Destructor and disposal logic
/// - Connection.cs: Connection management methods
/// - Fields.cs: Private field declarations
/// - Methods.cs: Core database operation methods
/// - Models.cs: Internal model definitions
/// - MongoHelper.cs: Main class definition
/// - Transactions.cs: Transaction management methods
/// - Utils.cs: Utility and helper methods
/// </para>
///
/// Example:
/// <code>
/// using var db = new MongoHelper("mongodb://localhost:27017");
/// db.Connect()
///   .TransactionBegin()
///   .ExecuteUpsert("users",
///       Builders&lt;BsonDocument&gt;.Filter.Eq("_id", 1),
///       Builders&lt;BsonDocument&gt;.Update.Set("name", "John"),
///       out var result)
///   .TransactionCommit()
///   .Disconnect();
/// </code>
/// </remarks>
/// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/"/>
/// <seealso href="https://www.mongodb.com/docs/manual/reference/connection-string/"/>
/// <seealso cref="System.IDisposable"/>
/// <seealso cref="System.IAsyncDisposable"/>
public sealed partial class MongoHelper : IDisposable, IAsyncDisposable { }
