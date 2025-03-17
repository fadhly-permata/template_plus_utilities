namespace IDC.Utilities.Data;

/// <summary>
/// Provides helper methods for PostgreSQL database operations with transaction support.
/// </summary>
/// <remarks>
/// <para>
/// The PostgreHelper class provides a comprehensive set of methods for interacting with PostgreSQL databases, including:
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
/// - PostgreHelper.cs: Main class definition
/// - Transaction.cs: Transaction management methods
/// - Utils.cs: Utility and helper methods
/// </para>
/// </remarks>
/// <example>
/// Basic Usage:
/// <code>
/// using var db = new PostgreHelper("Host=localhost;Database=mydb;Username=user;Password=pass");
/// db.Connect()
///   .ExecuteQuery("SELECT * FROM Users", out List&lt;JObject&gt; users)
///   .Disconnect();
/// </code>
///
/// Transaction Usage:
/// <code>
/// using var db = new PostgreHelper("Host=localhost;Database=mydb;Username=user;Password=pass");
/// db.Connect()
///   .TransactionBegin()
///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out int rows)
///   .TransactionCommit()
///   .Disconnect();
/// </code>
/// </example>
/// <seealso href="https://www.npgsql.org/doc/connection-string-parameters.html">PostgreSQL Connection Strings</seealso>
public sealed partial class PostgreHelper : IDisposable { }
