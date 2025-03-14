namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Contains customizable messages for SQLite operations.
    /// </summary>
    /// <remarks>
    /// Provides customizable text messages for various database operations and states.
    /// Messages can be modified to support different languages or custom error handling.
    /// </remarks>
    /// <example>
    /// <code>
    /// var db = new SQLiteHelper("Data Source=mydb.sqlite");
    /// db.Messages.MSG_CONNECTION_ESTABLISHED = "Database connected successfully";
    /// </code>
    /// </example>
    public class Messages
    {
        /// <summary>
        /// Message for successful database connection.
        /// </summary>
        public string MSG_CONNECTION_ESTABLISHED { get; set; } =
            "The connection to the SQLite database has been established.";

        /// <summary>
        /// Message for failed database connection.
        /// </summary>
        public string MSG_CONNECTION_NOT_ESTABLISHED { get; set; } =
            "The connection to the SQLite database has not been established.";

        /// <summary>
        /// Message for existing transaction.
        /// </summary>
        public string MSG_TRANSACTION_STARTED { get; set; } =
            "The database transaction has already commenced.";

        /// <summary>
        /// Message for missing transaction.
        /// </summary>
        public string MSG_TRANSACTION_NOT_STARTED { get; set; } =
            "The database transaction has not been initiated.";

        /// <summary>
        /// Message for disposed instance access.
        /// </summary>
        public string MSG_ALREADY_DISPOSED { get; set; } =
            "The SQLiteHelper has already been disposed.";
    }
}
