namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    public class Messages
    {
        public string MSG_CONNECTION_ESTABLISHED { get; set; } =
            "The connection to the PostgreSQL database has been established.";
        public string MSG_CONNECTION_NOT_ESTABLISHED { get; set; } =
            "The connection to the PostgreSQL database has not been established.";
        public string MSG_TRANSACTION_STARTED { get; set; } =
            "The database transaction has already commenced.";
        public string MSG_TRANSACTION_NOT_STARTED { get; set; } =
            "The database transaction has not been initiated.";
        public string MSG_ALREADY_DISPOSED { get; set; } =
            "The PostgreHelper has already been disposed.";
    }
}
