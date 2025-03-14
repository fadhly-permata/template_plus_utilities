using IDC.Utilities.Models.Data;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    public PostgreHelper(string connectionString, SystemLogging? logging = null)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            _logging = logging;
            _connection = new(connectionString: connectionString);
            _connection.Open();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper(CommonConnectionString connectionString, SystemLogging? logging = null)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(argument: connectionString);

            _logging = logging;
            _connection = new(connectionString: connectionString.ToPostgreSQL());
            _connection.Open();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
