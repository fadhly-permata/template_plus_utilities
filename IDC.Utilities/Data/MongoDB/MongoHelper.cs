namespace IDC.Utilities.Data;

/// <summary>
/// Provides helper methods for MongoDB database operations with transaction support.
/// </summary>
/// <remarks>
/// <para>
/// The MongoHelper class provides a comprehensive set of methods for interacting with MongoDB databases, including:
/// - Connection management (Connect, Disconnect, Reconnect)
/// - Transaction handling (Begin, Commit, Rollback)
/// - Query execution (InsertOne, UpdateOne, DeleteOne, Find)
/// - Connection state validation
/// </para>
///
/// <para>
/// Key Features:
/// - Thread-safe database operations
/// - Automatic connection and transaction management
/// - Support for JSON-formatted results
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
/// - Transaction.cs: Transaction management methods
/// - Utils.cs: Utility and helper methods
/// </para>
/// </remarks>
/// <example>
/// Basic Usage:
/// <code>
/// using var db = new MongoHelper("mongodb://localhost:27017", "mydb");
/// db.Connect()
///   .Find("users", filter: "{}", out List&lt;JObject&gt; users)
///   .Disconnect();
/// </code>
///
/// Transaction Usage:
/// <code>
/// using var db = new MongoHelper("mongodb://localhost:27017", "mydb");
/// db.Connect()
///   .TransactionBegin()
///   .InsertOne("users", document: new JObject { ["name"] = "John" }, out _)
///   .TransactionCommit()
///   .Disconnect();
/// </code>
/// </example>
public sealed partial class MongoHelper : IDisposable, IAsyncDisposable { }
