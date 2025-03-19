namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Contains predefined messages for various MongoDB operations and states.
    /// </summary>
    /// <remarks>
    /// This class provides a centralized location for storing and managing messages
    /// related to MongoDB operations, connection states, and error conditions.
    ///
    /// <para>
    /// Example usage:
    /// <code>
    /// var messages = new MongoHelper.Messages();
    /// Console.WriteLine(messages.MSG_CONNECTION_ESTABLISHED);
    /// </code>
    /// </para>
    /// </remarks>
    public class Messages
    {
        /// <summary>
        /// Gets or sets the message indicating a successful MongoDB connection.
        /// </summary>
        /// <value>A string message confirming the database connection.</value>
        public string MSG_CONNECTION_ESTABLISHED { get; set; } =
            "The connection to the MongoDB database has been established.";

        /// <summary>
        /// Gets or sets the message indicating a failed MongoDB connection.
        /// </summary>
        /// <value>A string message stating that the database connection was not established.</value>
        public string MSG_CONNECTION_NOT_ESTABLISHED { get; set; } =
            "The connection to the MongoDB database has not been established.";

        /// <summary>
        /// Gets or sets the message indicating an already started transaction.
        /// </summary>
        /// <value>A string message confirming that a database transaction is in progress.</value>
        public string MSG_TRANSACTION_STARTED { get; set; } =
            "The database transaction has already commenced.";

        /// <summary>
        /// Gets or sets the message indicating that no transaction has been started.
        /// </summary>
        /// <value>A string message stating that no database transaction has been initiated.</value>
        public string MSG_TRANSACTION_NOT_STARTED { get; set; } =
            "The database transaction has not been initiated.";

        /// <summary>
        /// Gets or sets the message indicating that the MongoHelper instance has been disposed.
        /// </summary>
        /// <value>A string message stating that the MongoHelper instance is no longer available.</value>
        public string MSG_ALREADY_DISPOSED { get; set; } =
            "The MongoHelper has already been disposed.";
    }
}
