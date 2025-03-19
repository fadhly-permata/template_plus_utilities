namespace IDC.Utilities.Data;

/// <summary>
/// Provides helper methods for MongoDB database operations with transaction support.
/// </summary>
/// <remarks>
/// The MongoHelper class provides a comprehensive set of methods for interacting with MongoDB databases.
///
/// Key Features:
///
/// > [!NOTE]
/// > All operations are thread-safe and support method chaining for fluent API usage
///
/// * Thread-safe database operations
/// * Automatic connection and transaction management
/// * Support for JSON-formatted results using <see cref="Newtonsoft.Json.Linq.JObject"/>
/// * Customizable logging and error handling
/// * Method chaining support for fluent API usage
///
/// Core Functionality:
///
/// * Connection management (<see cref="Connect"/>, <see cref="Disconnect"/>, <see cref="Reconnect"/>)
/// * Transaction handling (<see cref="TransactionBegin"/>, <see cref="TransactionCommit"/>, <see cref="TransactionRollback"/>)
/// * Query execution (<see cref="InsertOne"/>, <see cref="UpdateOne"/>, <see cref="DeleteOne"/>, <see cref="Find"/>)
/// * Connection state validation
///
/// File Structure:
///
/// |File|Description|
/// |---|---|
/// |CTOR.cs|Constructor implementations|
/// |DTOR.cs|Destructor and disposal logic|
/// |Connection.cs|Connection management methods|
/// |Fields.cs|Private field declarations|
/// |Methods.cs|Core database operation methods|
/// |Models.cs|Internal model definitions|
/// |MongoHelper.cs|Main class definition|
/// |Transaction.cs|Transaction management methods|
/// |Utils.cs|Utility and helper methods|
///
/// Example Usage:
/// <code>
/// // Basic operations
/// using var db = new MongoHelper("mongodb://localhost:27017", "mydb");
/// db.Connect()
///   .Find("users", filter: "{}", out List&lt;JObject&gt; users)
///   .Disconnect();
///
/// // Transaction operations
/// using var db = new MongoHelper("mongodb://localhost:27017", "mydb");
/// db.Connect()
///   .TransactionBegin()
///   .InsertOne("users", document: new JObject { ["name"] = "John" }, out _)
///   .TransactionCommit()
///   .Disconnect();
/// </code>
/// </remarks>
/// <seealso href="https://docs.mongodb.com/drivers/csharp/">MongoDB .NET Driver Documentation</seealso>
/// <seealso href="https://www.mongodb.com/docs/manual/core/transactions/">MongoDB Transactions</seealso>
/// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject</seealso>
public sealed partial class MongoHelper : IDisposable, IAsyncDisposable { }
