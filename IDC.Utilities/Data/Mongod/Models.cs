namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Contains customizable messages for MongoDB operations.
    /// </summary>
    /// <remarks>
    /// Provides customizable text messages for various database operations and states.
    /// Messages can be modified to support different languages or custom error handling.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// mongo.Messages.MSG_CONNECTION_ESTABLISHED = "Successfully connected to MongoDB";
    /// </code>
    /// </remarks>
    public class Messages
    {
        /// <summary>
        /// Message indicating successful database connection establishment.
        /// </summary>
        /// <value>Default: "The connection to the MongoDB database has been established."</value>
        public string MSG_CONNECTION_ESTABLISHED { get; set; } =
            "The connection to the MongoDB database has been established.";

        /// <summary>
        /// Message indicating failed database connection attempt.
        /// </summary>
        /// <value>Default: "The connection to the MongoDB database has not been established."</value>
        public string MSG_CONNECTION_NOT_ESTABLISHED { get; set; } =
            "The connection to the MongoDB database has not been established.";

        /// <summary>
        /// Message indicating an attempt to start a transaction when one is already active.
        /// </summary>
        /// <value>Default: "The database transaction has already commenced."</value>
        public string MSG_TRANSACTION_STARTED { get; set; } =
            "The database transaction has already commenced.";

        /// <summary>
        /// Message indicating an attempt to perform transaction operations without an active transaction.
        /// </summary>
        /// <value>Default: "The database transaction has not been initiated."</value>
        public string MSG_TRANSACTION_NOT_STARTED { get; set; } =
            "The database transaction has not been initiated.";

        /// <summary>
        /// Message indicating an attempt to use a disposed MongoHelper instance.
        /// </summary>
        /// <value>Default: "The MongoHelper has already been disposed."</value>
        /// <seealso cref="MongoDB.Driver.IMongoClient"/>
        /// <seealso cref="System.IDisposable"/>
        public string MSG_ALREADY_DISPOSED { get; set; } =
            "The MongoHelper has already been disposed.";
    }
}
