namespace IDC.Utilities.Data;

/// <summary>
/// Provides helper methods for SQLite database operations with transaction support.
/// </summary>
/// <remarks>
/// <para>
/// The SQLiteHelper class provides a comprehensive set of methods for interacting with SQLite databases, including:
/// - Connection management (Connect, Disconnect, Reconnect)
/// - Transaction handling (Begin, Commit, Rollback)
/// - Query execution (ExecuteNonQuery, ExecuteScalar, ExecuteReader, ExecuteQuery)
/// - Connection state validation
/// </para>
///
/// <para>
/// Key Features:
/// - Thread-safe database operations
/// - Automatic connection and transaction management
/// - Support for both standard queries and JSON-formatted results
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
/// - SQLiteHelper.cs: Main class definition
/// - Transaction.cs: Transaction management methods
/// - Utils.cs: Utility and helper methods
/// </para>
/// </remarks>
/// <example>
/// Basic Usage:
/// <code>
/// using var db = new SQLiteHelper("Data Source=database.db");
/// db.Connect()
///   .ExecuteQuery("SELECT * FROM Users", out List&lt;JObject&gt; users)
///   .Disconnect();
/// </code>
///
/// Transaction Usage:
/// <code>
/// using var db = new SQLiteHelper("Data Source=database.db");
/// db.Connect()
///   .TransactionBegin()
///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out int rows)
///   .TransactionCommit()
///   .Disconnect();
/// </code>
/// </example>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings">SQLite Connection Strings</seealso>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/transactions">SQLite Transactions</seealso>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/async">SQLite Async Operations</seealso>
public sealed partial class SQLiteHelper : IDisposable { }
