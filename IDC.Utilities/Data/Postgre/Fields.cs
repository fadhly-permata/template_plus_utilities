using Npgsql;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    private readonly SystemLogging? _logging;
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _dbTrans;
    private Messages _messages = new();
    private bool _disposed;
}
