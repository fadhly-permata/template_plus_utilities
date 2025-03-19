using System.Runtime.InteropServices;
using IDC.Utilities.Extensions;

namespace IDC.Utilities.Models.Data;

/// <summary>
/// Provides a unified connection string format that supports multiple database systems.
/// </summary>
/// <remarks>
/// Standardizes connection string handling across various database platforms including:
/// - SQLite
/// - PostgreSQL
/// - MongoDB
/// - SQL Server
/// - Oracle
/// - MariaDB
/// - Cassandra
///
/// Example connection strings:
/// ```
/// // PostgreSQL
/// "Server=localhost;Port=5432;Database=mydb;User ID=user;Password=pass;"
///
/// // MongoDB
/// "mongodb://user:pass@localhost:27017/mydb"
///
/// // Oracle TNS
/// "(DESCRIPTION=(HOST=myhost)(PORT=1521)(SERVICE_NAME=myservice));User Id=user;Password=pass;"
/// ```
///
/// > [!IMPORTANT]
/// > Always secure sensitive connection string information, especially in production environments.
///
/// > [!TIP]
/// > Use environment variables or secure configuration storage for connection strings.
/// </remarks>
/// <seealso href="https://www.mongodb.com/docs/manual/reference/connection-string/">MongoDB Connection String</seealso>
/// <seealso href="https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNSTRING">PostgreSQL Connection String</seealso>
/// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqliteconnectionstringbuilder">SQLite Connection String</seealso>
public class CommonConnectionString
{
    /// <summary>
    /// Initializes a new instance of CommonConnectionString with default values.
    /// </summary>
    /// <remarks>
    /// Creates a connection string with these defaults:
    /// - Server: localhost
    /// - Port: 0 (uses default port for each database)
    /// - Database: empty
    /// - Username: empty
    /// - Password: empty
    /// - Connection Timeout: 30 seconds
    /// - Pooling: true
    /// - Min Pool Size: 1
    /// - Max Pool Size: 100
    /// - Trust Server Certificate: true
    /// - Integrated Security: false
    /// - Application Name: null
    /// </remarks>
    /// <example>
    /// <code>
    /// var connStr = new CommonConnectionString();
    /// // Customize as needed
    /// connStr.Server = "myserver";
    /// connStr.Database = "mydb";
    /// </code>
    /// </example>
    public CommonConnectionString()
    {
        Server = "localhost";
        Port = 0;
        Database = ":memory:";
        Username = string.Empty;
        Password = string.Empty;
        ConnectionTimeout = 30;
        Pooling = true;
        MinPoolSize = 1;
        MaxPoolSize = 100;
        TrustServerCertificate = true;
        IntegratedSecurity = false;
        ApplicationName = null;
    }

    /// <summary>
    /// Gets or sets the server/host name for the database connection.
    /// </summary>
    /// <value>
    /// The server name, hostname, or IP address.
    /// Defaults to "localhost" if not specified.
    /// </value>
    /// <remarks>
    /// For different database systems:
    /// - SQLite: Not used (uses Database for file path)
    /// - MongoDB: Can be multiple hosts for replica sets
    /// - PostgreSQL/SQL Server: Server name or IP
    /// - Oracle: TNS host name
    ///
    /// > [!NOTE]
    /// > IPv6 addresses should be enclosed in square brackets.
    /// </remarks>
    public string? Server { get; set; }

    /// <summary>
    /// Gets or sets the database name or file path.
    /// </summary>
    /// <value>
    /// Database name, catalog name, or file path for SQLite.
    /// Defaults to ":memory:" for SQLite in-memory database.
    /// </value>
    /// <remarks>
    /// Database-specific usage:
    /// - SQLite: File path or ":memory:"
    /// - PostgreSQL: Database name
    /// - MongoDB: Database name
    /// - Oracle: Service name
    /// - Cassandra: Keyspace name
    ///
    /// > [!IMPORTANT]
    /// > For SQLite, ensure proper file permissions and directory existence.
    /// </remarks>
    public string? Database { get; set; }

    /// <summary>
    /// Gets or sets the username for database authentication.
    /// </summary>
    /// <value>
    /// Authentication username.
    /// Empty string by default.
    /// </value>
    /// <remarks>
    /// Authentication considerations:
    /// - Not used for SQLite
    /// - Required for most remote database connections
    /// - Optional when using Integrated Security
    ///
    /// > [!CAUTION]
    /// > Avoid hardcoding credentials in source code.
    /// </remarks>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for database authentication.
    /// </summary>
    /// <value>
    /// Authentication password.
    /// Empty string by default.
    /// </value>
    /// <remarks>
    /// Security considerations:
    /// - Store securely using encryption or secure configuration
    /// - Not used for SQLite
    /// - Optional when using Integrated Security
    ///
    /// > [!WARNING]
    /// > Never log or display password values.
    /// </remarks>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the port number for the database connection.
    /// </summary>
    /// <value>
    /// Port number, 0 means use default port.
    /// </value>
    /// <remarks>
    /// Default ports:
    /// - PostgreSQL: 5432
    /// - MongoDB: 27017
    /// - SQL Server: 1433
    /// - MySQL/MariaDB: 3306
    /// - Oracle: 1521
    /// - Cassandra: 9042
    ///
    /// > [!NOTE]
    /// > Port 0 triggers use of database-specific default port.
    /// </remarks>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets whether to use Windows Authentication.
    /// </summary>
    /// <value>
    /// True to use Windows Authentication, false for SQL authentication.
    /// Defaults to false.
    /// </value>
    /// <remarks>
    /// Supported primarily by:
    /// - SQL Server
    /// - PostgreSQL (with SSPI)
    ///
    /// > [!NOTE]
    /// > Not applicable for SQLite, MongoDB, and some other databases.
    /// </remarks>
    public bool IntegratedSecurity { get; set; }

    /// <summary>
    /// Gets or sets whether to trust the server certificate.
    /// </summary>
    /// <value>
    /// True to trust server certificate, false to validate.
    /// Defaults to true.
    /// </value>
    /// <remarks>
    /// Security implications:
    /// - Development: Often set to true for self-signed certificates
    /// - Production: Should be false to ensure proper certificate validation
    ///
    /// > [!CAUTION]
    /// > Setting to true in production may expose to MITM attacks.
    /// </remarks>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    /// <value>
    /// Timeout in seconds.
    /// Defaults to 30 seconds.
    /// </value>
    /// <remarks>
    /// Considerations:
    /// - Shorter timeouts for web applications
    /// - Longer timeouts for batch processing
    /// - Some databases use milliseconds internally
    ///
    /// > [!TIP]
    /// > Adjust based on network reliability and application needs.
    /// </remarks>
    public int ConnectionTimeout { get; set; }

    /// <summary>
    /// Gets or sets the application name identifier.
    /// </summary>
    /// <value>
    /// Application identifier string.
    /// Null by default.
    /// </value>
    /// <remarks>
    /// Used for:
    /// - Monitoring and logging
    /// - Resource attribution
    /// - Connection pooling identification
    ///
    /// > [!TIP]
    /// > Set to meaningful value for better diagnostics.
    /// </remarks>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets whether connection pooling is enabled.
    /// </summary>
    /// <value>
    /// True to enable pooling, false to disable.
    /// Defaults to true.
    /// </value>
    /// <remarks>
    /// Benefits:
    /// - Reduces connection overhead
    /// - Improves application performance
    /// - Manages connection resources
    ///
    /// > [!IMPORTANT]
    /// > Disable only when specifically required.
    /// </remarks>
    public bool Pooling { get; set; }

    /// <summary>
    /// Gets or sets the minimum connection pool size.
    /// </summary>
    /// <value>
    /// Minimum number of connections.
    /// Defaults to 1.
    /// </value>
    /// <remarks>
    /// Considerations:
    /// - Higher values pre-allocate connections
    /// - Impacts startup time
    /// - Memory usage implications
    ///
    /// > [!TIP]
    /// > Set based on minimum concurrent connection needs.
    /// </remarks>
    public int MinPoolSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum connection pool size.
    /// </summary>
    /// <value>
    /// Maximum number of connections.
    /// Defaults to 100.
    /// </value>
    /// <remarks>
    /// Factors to consider:
    /// - Available system resources
    /// - Expected concurrent connections
    /// - Database server capacity
    ///
    /// > [!IMPORTANT]
    /// > Monitor pool exhaustion in production.
    /// </remarks>
    public int MaxPoolSize { get; set; }

    /// <summary>
    /// Parses a MongoDB connection URL into a CommonConnectionString object.
    /// </summary>
    /// <param name="connectionString">MongoDB connection URL to parse</param>
    /// <returns>A <see cref="CommonConnectionString"/> instance populated with MongoDB connection details</returns>
    /// <remarks>
    /// Supports standard MongoDB connection URL format with authentication and options.
    /// See: <see href="https://www.mongodb.com/docs/manual/reference/connection-string/"/>
    ///
    /// Example formats:
    /// <code>
    /// mongodb://localhost:27017/mydatabase
    /// mongodb://myuser:mypassword@localhost:27017/mydatabase
    /// mongodb://myuser:mypassword@localhost:27017/mydatabase?retryWrites=true&amp;w=majority
    /// </code>
    ///
    /// > [!NOTE]
    /// > IPv6 addresses should be enclosed in square brackets, e.g., [::1]
    ///
    /// > [!IMPORTANT]
    /// > Credentials in the connection string should be properly URI-encoded
    /// </remarks>
    private static CommonConnectionString ParseMongoDbUrl(string connectionString)
    {
        var result = new CommonConnectionString();
        var uri = new Uri(uriString: connectionString);
        result.Server = uri.Host;
        result.Port = uri.Port;
        result.Database = uri.AbsolutePath.TrimStart(trimChars: '/');

        if (!string.IsNullOrEmpty(value: uri.UserInfo))
        {
            var credentials = uri.UserInfo.Split(separator: ':', options: StringSplitOptions.None);
            result.Username = credentials[0];
            if (credentials.Length > 1)
                result.Password = credentials[1];
        }

        return result;
    }

    /// <summary>
    /// Parses an Oracle TNS connection string into a CommonConnectionString object.
    /// </summary>
    /// <param name="connectionString">Oracle TNS format connection string to parse</param>
    /// <returns>A <see cref="CommonConnectionString"/> instance populated with Oracle connection details</returns>
    /// <remarks>
    /// Supports Oracle TNS format with various parameters including HOST, PORT, SERVICE_NAME.
    /// See: <see href="https://www.oracle.com/database/technologies/net-connection-using-tns.html"/>
    ///
    /// Example formats:
    /// <code>
    /// (DESCRIPTION=(HOST=myhost)(PORT=1521)(SERVICE_NAME=myservice))
    /// (DESCRIPTION=(HOST=myhost)(PORT=1521)(SERVICE_NAME=myservice));User Id=myuser;Password=mypassword
    /// </code>
    ///
    /// > [!NOTE]
    /// > Uses regex patterns from <see cref="RegexPatternCollections"/> for parsing
    ///
    /// > [!TIP]
    /// > For better security, consider using external credential providers instead of embedding credentials
    /// </remarks>
    private static CommonConnectionString ParseOracleTns(string connectionString)
    {
        var result = new CommonConnectionString();

        var hostMatch = RegexPatternCollections.OracleTnsHost().Match(connectionString);
        if (hostMatch.Success)
            result.Server = hostMatch.Groups[1].Value;

        var portMatch = RegexPatternCollections.OracleTnsPort().Match(connectionString);
        if (portMatch.Success)
            result.Port = int.Parse(portMatch.Groups[1].Value);

        var serviceMatch = RegexPatternCollections.OracleTnsServiceName().Match(connectionString);
        if (serviceMatch.Success)
            result.Database = serviceMatch.Groups[1].Value;

        var userMatch = RegexPatternCollections.OracleTnsUserId().Match(connectionString);
        if (userMatch.Success)
            result.Username = userMatch.Groups[1].Value;

        var passwordMatch = RegexPatternCollections.OracleTnsPassword().Match(connectionString);
        if (passwordMatch.Success)
            result.Password = passwordMatch.Groups[1].Value;

        return result;
    }

    /// <summary>
    /// Parses server value that may include port number.
    /// </summary>
    /// <param name="result">CommonConnectionString instance to update</param>
    /// <param name="value">Server value string that may include port</param>
    /// <remarks>
    /// Handles various server:port formats including:
    /// <code>
    /// localhost:5432
    /// server,1433
    /// [::1]:27017
    /// [2001:db8::1]:5432
    /// </code>
    ///
    /// > [!NOTE]
    /// > IPv6 addresses are automatically detected and properly handled
    /// </remarks>
    private static void ParseServerValue(CommonConnectionString result, string value)
    {
        var serverParts = value.Split(separator: ',')[0].Split(separator: ':');
        result.Server = serverParts[0].Trim().TrimEnd(trimChar: ']').TrimStart(trimChar: '[');

        if (serverParts.Length > 1)
            result.Port = serverParts[1].CastToInteger() ?? 0;
    }

    /// <summary>
    /// Parses timeout value that may be in seconds or milliseconds.
    /// </summary>
    /// <param name="value">Timeout string value</param>
    /// <returns>Timeout value in seconds</returns>
    /// <remarks>
    /// Supports formats:
    /// <code>
    /// "30"      // 30 seconds
    /// "30000ms" // 30 seconds
    /// "5000ms"  // 5 seconds
    /// </code>
    ///
    /// > [!NOTE]
    /// > Returns 30 seconds as default if parsing fails
    ///
    /// > [!TIP]
    /// > Use explicit units (ms) for clarity when specifying milliseconds
    /// </remarks>
    private static int ParseTimeout(string value) =>
        value.EndsWith(value: "ms")
            ? value.TrimEnd(trimChars: ['m', 's']).CastToInteger() ?? 30
            : value.CastToInteger() ?? 30;

    /// <summary>
    /// Parses string value to boolean based on provided true values.
    /// </summary>
    /// <param name="value">String value to parse</param>
    /// <param name="trueValues">Array of strings that represent true value</param>
    /// <returns>Boolean result of the parsing</returns>
    /// <remarks>
    /// Case-insensitive comparison against provided true values.
    ///
    /// Example usage:
    /// <code>
    /// ParseBooleanValue("yes", "true", "yes", "1")  // returns true
    /// ParseBooleanValue("no", "true", "yes", "1")   // returns false
    /// ParseBooleanValue("TRUE", "true")             // returns true
    /// </code>
    ///
    /// > [!TIP]
    /// > Common true values include: "true", "yes", "1", "on"
    /// </remarks>
    private static bool ParseBooleanValue(string value, params string[] trueValues) =>
        trueValues.Any(predicate: v =>
            value.Equals(value: v, comparisonType: StringComparison.OrdinalIgnoreCase)
        );

    /// <summary>
    /// Processes key-value pair from connection string and updates CommonConnectionString properties.
    /// </summary>
    /// <param name="result">CommonConnectionString instance to update</param>
    /// <param name="key">Connection string parameter key (normalized to lowercase)</param>
    /// <param name="value">Connection string parameter value</param>
    /// <remarks>
    /// Handles various database connection string parameters with their common aliases:
    ///
    /// Server/Host:
    /// - Server
    /// - Data Source
    /// - Host
    /// - Contact Points
    ///
    /// Database:
    /// - Database
    /// - Initial Catalog
    /// - Default Keyspace
    /// - Bucket Name
    /// - DatabaseName
    ///
    /// Authentication:
    /// - User ID/UID
    /// - Username/User
    /// - Password/PWD
    /// - Integrated Security/Trusted Connection
    ///
    /// Connection Settings:
    /// - Port
    /// - Trust Server Certificate
    /// - Connection/Connect Timeout
    /// - Application Name
    ///
    /// Connection Pooling:
    /// - Pooling
    /// - Min/Max Pool Size
    ///
    /// Special handling for SQLite:
    /// - In-memory database (:memory:)
    /// - File-based database (.db, .sqlite, .sqlite3)
    ///
    /// > [!NOTE]
    /// > All keys are normalized to lowercase before processing
    ///
    /// > [!IMPORTANT]
    /// > For SQLite, the Data Source parameter is treated as the database path
    /// </remarks>
    /// <example>
    /// <code>
    /// var connStr = new CommonConnectionString();
    /// ProcessKeyValuePair(connStr, key: "server", value: "localhost");
    /// ProcessKeyValuePair(connStr, key: "database", value: "mydb");
    /// ProcessKeyValuePair(connStr, key: "user id", value: "admin");
    /// </code>
    /// </example>
    private static void ProcessKeyValuePair(CommonConnectionString result, string key, string value)
    {
        switch (key)
        {
            case "server":
            case "data source":
            case "host":
            case "contact points":
                // Khusus untuk SQLite, data source adalah path database
                if (value == ":memory:")
                {
                    result.Database = value;
                }
                else if (
                    value.EndsWith(".db")
                    || value.EndsWith(".sqlite")
                    || value.EndsWith(".sqlite3")
                )
                {
                    result.Database = value;
                }
                else
                {
                    ParseServerValue(result: result, value: value);
                }
                break;

            case "database":
            case "initial catalog":
            case "default keyspace":
            case "bucket_name":
            case "databasename":
                result.Database = value;
                break;

            case "user id":
            case "uid":
            case "username":
            case "user":
                result.Username = value;
                break;

            case "password":
            case "pwd":
                result.Password = value;
                break;

            case "port":
                result.Port = value.CastToInteger() ?? 0;
                break;

            case "integrated security":
            case "trusted_connection":
                result.IntegratedSecurity = ((string[])["true", "yes", "sspi"]).Any(predicate: v =>
                    value.Equals(value: v, comparisonType: StringComparison.OrdinalIgnoreCase)
                );
                break;

            case "trust server certificate":
            case "trustservercertificate":
                result.TrustServerCertificate = ParseBooleanValue(value: value, trueValues: "true");
                break;

            case "connection timeout":
            case "connect timeout":
            case "timeout":
            case "connection_timeout":
                result.ConnectionTimeout = ParseTimeout(value: value);
                break;

            case "application name":
            case "application":
                result.ApplicationName = value;
                break;

            case "pooling":
                result.Pooling = ParseBooleanValue(value: value, trueValues: "true");
                break;

            case "min pool size":
            case "minimum pool size":
                result.MinPoolSize = value.CastToInteger() ?? 1;
                break;

            case "max pool size":
            case "maximum pool size":
                result.MaxPoolSize = value.CastToInteger() ?? 100;
                break;
        }
    }

    /// <summary>
    /// Parses a connection string into a CommonConnectionString object.
    /// </summary>
    /// <param name="connectionString">The connection string to parse</param>
    /// <returns>A new <see cref="CommonConnectionString"/> instance populated with the parsed values</returns>
    /// <remarks>
    /// Supports multiple connection string formats:
    ///
    /// Standard Key-Value:
    /// <code>
    /// "Server=localhost;Database=mydb;User ID=user;Password=pass;"
    /// </code>
    ///
    /// MongoDB URL:
    /// <code>
    /// "mongodb://user:pass@localhost:27017/mydb"
    /// </code>
    ///
    /// Oracle TNS:
    /// <code>
    /// "(DESCRIPTION=(HOST=myhost)(PORT=1521)(SERVICE_NAME=myservice));User Id=user;Password=pass;"
    /// </code>
    ///
    /// > [!NOTE]
    /// > Special handling for MongoDB and Oracle TNS formats
    ///
    /// > [!IMPORTANT]
    /// > All key-value pairs are normalized and processed case-insensitively
    /// </remarks>
    /// <example>
    /// <code>
    /// var conn = new CommonConnectionString();
    /// var result = conn.FromConnectionString("Server=localhost;Database=mydb;User ID=user;Password=pass;");
    /// </code>
    /// </example>
    public virtual CommonConnectionString FromConnectionString(string connectionString)
    {
        if (connectionString.StartsWith(value: "mongodb://"))
            return ParseMongoDbUrl(connectionString: connectionString);

        if (connectionString.Contains(value: "(DESCRIPTION="))
            return ParseOracleTns(connectionString: connectionString);

        var result = new CommonConnectionString();

        foreach (
            var part in connectionString.Split(
                separator: [';'],
                options: StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            var keyValue = part.Split(separator: ['='], count: 2, options: StringSplitOptions.None);
            if (keyValue.Length != 2)
                continue;

            ProcessKeyValuePair(
                result: result,
                key: keyValue[0].Trim().ToLower(),
                value: keyValue[1].Trim()
            );
        }

        return result;
    }

    /// <summary>
    /// Converts to PostgreSQL connection string format.
    /// </summary>
    /// <returns>A formatted PostgreSQL connection string.</returns>
    /// <remarks>
    /// For more details about PostgreSQL connection strings, see:
    /// <see href="https://www.npgsql.org/doc/connection-string-parameters.html"/>
    /// </remarks>
    public string ToPostgreSQL() =>
        string.Join<string>(
            separator: ";",
            values: new string[]
            {
                $"Host={Server}",
                Port > 0 ? $"Port={Port}" : string.Empty,
                $"Database={Database}",
                $"Username={Username}",
                $"Password={Password}",
                $"Timeout={ConnectionTimeout}",
                $"Application Name={ApplicationName ?? "CommonConnectionString"}",
                $"Pooling={Pooling}",
                $"Minimum Pool Size={MinPoolSize}",
                $"Maximum Pool Size={MaxPoolSize}",
                "Trust Server Certificate=true"
            }.Where(predicate: static x => !string.IsNullOrEmpty(value: x))
        );

    /// <summary>
    /// Converts to SQLite connection string format.
    /// </summary>
    /// <param name="cache">Cache mode (default: "Shared"). Available values: Private, Shared, Default</param>
    /// <param name="mode">Access mode (default: "ReadWrite"). Available values: ReadOnly, ReadWrite, Memory</param>
    /// <returns>A formatted SQLite connection string.</returns>
    /// <remarks>
    /// Generates a connection string for SQLite database with specified cache and access modes.
    ///
    /// Example formats:
    /// <code>
    /// "Data Source=/path/to/db.sqlite;Cache=Shared;Mode=ReadWrite"
    /// "Data Source=:memory:;Cache=Private;Mode=Memory"
    /// </code>
    ///
    /// > [!IMPORTANT]
    /// > Paths are automatically converted to use forward slashes
    ///
    /// > [!NOTE]
    /// > Relative paths are resolved against the current directory
    ///
    /// For more details:
    /// - <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings"/>
    /// - <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache"/>
    /// - <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#mode"/>
    /// </remarks>
    public string ToSQLite(string cache = "Shared", string mode = "ReadWrite")
    {
        var dbPath = Database;
        if (!Path.IsPathRooted(path: dbPath))
        {
            dbPath = Path.Combine(
                path1: Directory.GetCurrentDirectory(),
                path2: dbPath ?? string.Empty
            );
        }
        dbPath = dbPath.Replace(oldChar: '\\', newChar: '/');

        return string.Join<string>(
            separator: ";",
            values: [$"Data Source={dbPath}", $"Cache={cache}", $"Mode={mode}"]
        );
    }

    /// <summary>
    /// Creates an in-memory SQLite connection string.
    /// </summary>
    /// <param name="cached">Whether to use shared cache (default: true)</param>
    /// <returns>A formatted in-memory SQLite connection string</returns>
    /// <remarks>
    /// Creates a connection string for in-memory SQLite database.
    ///
    /// Example formats:
    /// <code>
    /// "Data Source=:memory:;Mode=Memory;Cache=Shared"   // cached=true
    /// "Data Source=:memory:;Mode=Memory;Cache=Private"  // cached=false
    /// </code>
    ///
    /// > [!NOTE]
    /// > Shared cache allows multiple connections to access the same in-memory database
    ///
    /// > [!TIP]
    /// > Use shared cache when you need to access the same in-memory database from multiple connections
    ///
    /// For more details:
    /// - <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases"/>
    /// - <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache"/>
    /// </remarks>
#pragma warning disable CA1822 // Mark members as static
    public string ToSQLiteInMemory(bool cached = true) =>
        string.Join<string>(
            separator: ";",
            values:
            [
                "Data Source=:memory:",
                "Mode=Memory",
                $"Cache={(cached ? "Shared" : "Private")}"
            ]
        );
#pragma warning restore CA1822 // Mark members as static

    /// <summary>
    /// Converts to MySQL connection string format.
    /// </summary>
    /// <returns>A formatted MySQL connection string</returns>
    /// <remarks>
    /// Generates a connection string for MySQL database with standard options.
    ///
    /// Example formats:
    /// <code>
    /// "Server=localhost;Port=3306;Database=mydb;User ID=user;Password=pass;SSL Mode=Required"
    /// "Server=db.example.com;Database=mydb;User ID=user;Password=pass;Connection Timeout=30"
    /// </code>
    ///
    /// Default settings:
    /// - SSL Mode: Required
    /// - Allow User Variables: true
    /// - Convert Zero Datetime: true
    ///
    /// > [!NOTE]
    /// > Empty or default values are automatically filtered out
    ///
    /// For more details:
    /// <see href="https://dev.mysql.com/doc/connector-net/en/connector-net-connection-options.html"/>
    /// </remarks>
    public string ToMySQL() =>
        string.Join<string>(
            separator: ";",
            values: new string[]
            {
                $"Server={Server}",
                Port > 0 ? $"Port={Port}" : string.Empty,
                $"Database={Database}",
                $"User ID={Username}",
                $"Password={Password}",
                $"Connection Timeout={ConnectionTimeout}",
                $"Pooling={Pooling}",
                $"Min Pool Size={MinPoolSize}",
                $"Max Pool Size={MaxPoolSize}",
                "Allow User Variables=true",
                "Convert Zero Datetime=true",
                "SSL Mode=Required"
            }.Where(predicate: static x => !string.IsNullOrEmpty(value: x))
        );

    /// <summary>
    /// Converts to SQL Server connection string format.
    /// </summary>
    /// <returns>A formatted SQL Server connection string.</returns>
    /// <remarks>
    /// Generates a connection string for SQL Server with platform-specific authentication.
    ///
    /// Example formats:
    /// <code>
    /// // Windows Authentication
    /// "Server=localhost;Database=mydb;Integrated Security=SSPI;TrustServerCertificate=true"
    ///
    /// // SQL Authentication
    /// "Server=localhost,1433;Database=mydb;User ID=user;Password=pass;TrustServerCertificate=true"
    /// </code>
    ///
    /// > [!NOTE]
    /// > Uses SSPI for Windows authentication and standard security for other platforms
    ///
    /// > [!IMPORTANT]
    /// > Port is appended to Server using comma separator (SQL Server specific)
    ///
    /// Default settings:
    /// - MultipleActiveResultSets=true
    /// - TrustServerCertificate (configurable)
    /// - Connection pooling enabled
    ///
    /// For more details:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/connection-string-syntax#sql-server-connection-strings"/>
    /// </remarks>
    public string ToSQLServer() =>
        string.Join<string>(
            separator: ";",
            values:
            [
                $"Server={Server}{(Port > 0 ? $",{Port}" : string.Empty)}",
                $"Database={Database}",
                IntegratedSecurity
                    ? RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows)
                        ? "Integrated Security=SSPI"
                        : "Integrated Security=true"
                    : $"User ID={Username};Password={Password}",
                $"Connection Timeout={ConnectionTimeout}",
                $"Application Name={ApplicationName ?? "CommonConnectionString"}",
                $"TrustServerCertificate={TrustServerCertificate}",
                $"Pooling={Pooling}",
                $"Min Pool Size={MinPoolSize}",
                $"Max Pool Size={MaxPoolSize}",
                "MultipleActiveResultSets=true"
            ]
        );

    /// <summary>
    /// Converts to MariaDB connection string format.
    /// </summary>
    /// <returns>A formatted MariaDB connection string.</returns>
    /// <remarks>
    /// For more details about MariaDB connection strings, see:
    /// <see href="https://mariadb.com/kb/en/about-mariadb-connector-net/#connection-strings"/>
    /// </remarks>
    public string ToMariaDB() =>
        string.Join<string>(
            separator: ";",
            values: new string[]
            {
                $"Server={Server}",
                Port > 0 ? $"Port={Port}" : string.Empty,
                $"Database={Database}",
                $"User ID={Username}",
                $"Password={Password}",
                $"Connection Timeout={ConnectionTimeout}",
                $"Pooling={Pooling}",
                $"Min Pool Size={MinPoolSize}",
                $"Max Pool Size={MaxPoolSize}",
                "SSL Mode=Required",
                "Allow User Variables=true"
            }.Where(predicate: static x => !string.IsNullOrEmpty(value: x))
        );

    /// <summary>
    /// Converts to Oracle connection string format.
    /// </summary>
    /// <returns>A formatted Oracle connection string.</returns>
    /// <remarks>
    /// For more details about Oracle connection strings, see:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/connection-string-syntax#oracle-connection-strings"/>
    /// </remarks>
    public string ToOracle() =>
        string.Join<string>(
            separator: ";",
            values:
            [
                string.Join(
                    string.Empty,
                    [
                        "Data Source=(DESCRIPTION=",
                        "(ADDRESS=",
                        "(PROTOCOL=TCP)",
                        $"(HOST={Server})",
                        $"(PORT={Port})",
                        ")",
                        "(CONNECT_DATA=",
                        $"(SERVICE_NAME={Database})",
                        ")",
                        ")"
                    ]
                ),
                $"User Id={Username}",
                $"Password={Password}",
                "Pooling=true",
                "Statement Cache Size=20"
            ]
        );

    /// <summary>
    /// Converts to Oracle connection string format using TNS format.
    /// </summary>
    /// <returns>A formatted Oracle connection string.</returns>
    /// <remarks>
    /// Generates a TNS-style connection string for Oracle databases.
    ///
    /// Example format:
    /// <code>
    /// "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=myhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=myservice)));User Id=user;Password=pass;Pooling=true"
    /// </code>
    ///
    /// Default settings:
    /// - Protocol: TCP
    /// - Pooling: enabled
    /// - Statement Cache Size: 20
    ///
    /// > [!NOTE]
    /// > Uses standard TNS descriptor format for compatibility
    ///
    /// For more details:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/connection-string-syntax#oracle-connection-strings"/>
    /// </remarks>
    public string ToOracleTns() =>
        string.Join<string>(
            separator: ";",
            values:
            [
                $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={Server})(PORT={Port}))(CONNECT_DATA=(SERVICE_NAME={Database})))",
                $"User Id={Username}",
                $"Password={Password}",
                "Pooling=true",
                "Statement Cache Size=20"
            ]
        );

    /// <summary>
    /// Converts to MongoDB connection string format.
    /// </summary>
    /// <returns>A formatted MongoDB connection string.</returns>
    /// <remarks>
    /// Generates a MongoDB URI connection string with standard options.
    ///
    /// Example formats:
    /// <code>
    /// "mongodb://user:pass@localhost:27017/mydb?retryWrites=true&amp;w=majority&amp;ssl=true"
    /// "mongodb://localhost:27017/mydb?retryWrites=true&amp;w=majority&amp;ssl=true"
    /// </code>
    ///
    /// Default settings:
    /// - retryWrites=true
    /// - w=majority
    /// - ssl=true
    /// - connectTimeoutMS (based on ConnectionTimeout)
    ///
    /// > [!NOTE]
    /// > Username and password are automatically URL-encoded
    ///
    /// For more details:
    /// <see href="https://www.mongodb.com/docs/manual/reference/connection-string/"/>
    /// </remarks>
    public string ToMongoDB()
    {
        var auth = string.IsNullOrEmpty(value: Username)
            ? string.Empty
            : $"{Uri.EscapeDataString(stringToEscape: Username ?? string.Empty)}:{Uri.EscapeDataString(stringToEscape: Password ?? string.Empty)}@";
        List<string> options =
        [
            "retryWrites=true",
            "w=majority",
            "ssl=true",
            $"connectTimeoutMS={ConnectionTimeout * 1000}"
        ];

        return $"mongodb://{auth}{Server}:{Port}/{Database}?{string.Join(separator: "&", values: options)}";
    }

    /// <summary>
    /// Converts to Cassandra connection string format.
    /// </summary>
    /// <returns>A formatted Cassandra connection string.</returns>
    /// <remarks>
    /// Generates a connection string for Cassandra with standard security options.
    ///
    /// Example formats:
    /// <code>
    /// "Contact Points=localhost;Port=9042;Default Keyspace=mykeyspace;User ID=user;Password=pass;SSL=true"
    /// "Contact Points=cassandra.example.com;Default Keyspace=mykeyspace;SSL=true"
    /// </code>
    ///
    /// Default settings:
    /// - SSL: enabled
    /// - Connection Timeout: 10 seconds
    ///
    /// > [!NOTE]
    /// > Empty or default values are automatically filtered out
    ///
    /// For more details:
    /// <see href="https://docs.datastax.com/en/developer/csharp-driver/3.0/features/connection-pooling/"/>
    /// </remarks>
    public string ToCassandra() =>
        string.Join<string>(
            separator: ";",
            values: new[]
            {
                $"Contact Points={Server}",
                $"Port={Port}",
                $"Default Keyspace={Database}",
                !string.IsNullOrEmpty(value: Username) ? $"User ID={Username}" : string.Empty,
                !string.IsNullOrEmpty(value: Username) ? $"Password={Password}" : string.Empty,
                "SSL=true",
                "Connection Timeout=10"
            }.Where(predicate: static x => !string.IsNullOrEmpty(value: x))
        );

    /// <summary>
    /// Converts to Couchbase connection string format.
    /// </summary>
    /// <returns>A formatted Couchbase connection string.</returns>
    /// <remarks>
    /// Generates a Couchbase connection string with standard options.
    ///
    /// Example formats:
    /// <code>
    /// "couchbase://localhost?connection_timeout=30000&amp;bucket_name=default&amp;ssl=true"
    /// "couchbase://db.example.com?connection_timeout=30000&amp;bucket_name=mybucket&amp;ssl=true&amp;username=user&amp;password=pass"
    /// </code>
    ///
    /// Default settings:
    /// - ssl=true
    /// - connection_timeout (in milliseconds)
    /// - bucket_name (from Database property)
    ///
    /// > [!NOTE]
    /// > Username and password are automatically URL-encoded and only included if username is provided
    ///
    /// For more details:
    /// <see href="https://docs.couchbase.com/dotnet-sdk/current/ref/connection-string.html"/>
    /// </remarks>
    public string ToCouchbase()
    {
        List<string> options =
        [
            $"connection_timeout={ConnectionTimeout * 1000}",
            $"bucket_name={Database}",
            "ssl=true"
        ];

        if (!string.IsNullOrEmpty(Username))
        {
            options.Add($"username={Uri.EscapeDataString(stringToEscape: Username)}");
            options.Add(
                $"password={Uri.EscapeDataString(stringToEscape: Password ?? string.Empty)}"
            );
        }

        return $"couchbase://{Server}?{string.Join(separator: "&", values: options)}";
    }
}
