using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IDC.Utilities.Extensions;

namespace IDC.Utilities.Models.Data;

/// <summary>
/// Represents a common connection string format that can be converted to various database-specific formats.
/// </summary>
/// <remarks>
/// This class provides a standardized way to handle connection strings across different database systems.
/// It supports parsing from standard connection string format and conversion to specific database formats.
///
/// Example connection string:
/// "Server=localhost;Database=mydb;User ID=user;Password=pass;Port=5432;Connection Timeout=30"
/// </remarks>
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
    /// Gets or sets the server/host name.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// Gets or sets the database/catalog name.
    /// </summary>
    public string? Database { get; set; }

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the port number. Default is 0.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets whether to use Windows Authentication.
    /// </summary>
    public bool IntegratedSecurity { get; set; }

    /// <summary>
    /// Gets or sets whether to trust the server certificate.
    /// </summary>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the connection timeout in seconds. Default is 30.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30;

    /// <summary>
    /// Gets or sets the application name identifier.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets whether connection pooling is enabled. Default is true.
    /// </summary>
    public bool Pooling { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum pool size. Default is 1.
    /// </summary>
    public int MinPoolSize { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum pool size. Default is 100.
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Parses a MongoDB connection URL into a CommonConnectionString object.
    /// </summary>
    /// <param name="connectionString">MongoDB connection URL in format: mongodb://[username:password@]host[:port]/database</param>
    /// <returns>A CommonConnectionString instance populated with MongoDB connection details</returns>
    /// <remarks>
    /// Supports standard MongoDB connection URL format.
    /// See: <see href="https://www.mongodb.com/docs/manual/reference/connection-string/"/>
    ///
    /// Example:
    /// <code>
    /// mongodb://myuser:mypassword@localhost:27017/mydatabase
    /// </code>
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
    /// <param name="connectionString">Oracle TNS format connection string</param>
    /// <returns>A CommonConnectionString instance populated with Oracle connection details</returns>
    /// <remarks>
    /// Supports Oracle TNS format with HOST, PORT, SERVICE_NAME parameters.
    /// See: <see href="https://www.oracle.com/database/technologies/net-connection-using-tns.html"/>
    ///
    /// Example:
    /// <code>
    /// (DESCRIPTION=(HOST=myhost)(PORT=1521)(SERVICE_NAME=myservice));User Id=myuser;Password=mypassword
    /// </code>
    /// </remarks>
    private static CommonConnectionString ParseOracleTns(string connectionString)
    {
        var result = new CommonConnectionString();
        var patterns = new Dictionary<string, string>
        {
            { "Server", @"HOST=([^)]+)" },
            { "Port", @"PORT=(\d+)" },
            { "Database", @"SERVICE_NAME=([^)]+)" },
            { "Username", @"User Id=([^;]+)" },
            { "Password", @"Password=([^;]+)" }
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(input: connectionString, pattern: pattern.Value);
            if (!match.Success)
                continue;

            var value = match.Groups[1].Value;
            switch (pattern.Key)
            {
                case "Server":
                    result.Server = value;
                    break;
                case "Port":
                    result.Port = value.CastToInteger() ?? 0;
                    break;
                case "Database":
                    result.Database = value;
                    break;
                case "Username":
                    result.Username = value;
                    break;
                case "Password":
                    result.Password = value;
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Parses server value that may include port number.
    /// </summary>
    /// <param name="result">CommonConnectionString instance to update</param>
    /// <param name="value">Server value string that may include port (e.g. "localhost,1433" or "server:5432")</param>
    /// <remarks>
    /// Handles various server:port formats including IPv6 addresses.
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
    /// <param name="value">Timeout string value (e.g. "30" or "30000ms")</param>
    /// <returns>Timeout value in seconds</returns>
    /// <remarks>
    /// Converts milliseconds to seconds if "ms" suffix is present.
    /// Returns 30 seconds as default if parsing fails.
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
    /// <returns>True if value matches any trueValues (case-insensitive), false otherwise</returns>
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
    /// Supports various parameter aliases across different database systems:
    /// - Server/Data Source/Host/Contact Points
    /// - Database/Initial Catalog/Default Keyspace/Bucket Name
    /// - User ID/UID/Username/User
    /// - Password/PWD
    /// - Integrated Security/Trusted Connection
    /// - Connection Timeout/Connect Timeout/Timeout
    /// - Min/Max Pool Size variants
    /// </remarks>
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
    /// <param name="connectionString">The connection string to parse.</param>
    /// <returns>A new CommonConnectionString instance populated with the parsed values.</returns>
    /// <remarks>
    /// Supports common connection string formats with various key aliases:
    /// - Server/Data Source/Host
    /// - Database/Initial Catalog
    /// - User ID/UID/Username
    /// - Password/PWD
    /// - Port
    /// - Integrated Security
    /// - Trust Server Certificate
    /// - Connection/Connect Timeout
    /// - Application Name
    /// - Pooling
    /// - Min/Max Pool Size
    /// </remarks>
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
    /// For more details about SQLite connection strings, see:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings"/>
    ///
    /// Cache modes documentation:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache"/>
    ///
    /// Access modes documentation:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#mode"/>
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
    /// <param name="cached">Whether to use shared cache (default: true).</param>
    /// <returns>A formatted in-memory SQLite connection string.</returns>
    /// <remarks>
    /// For more details about SQLite in-memory databases, see:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases"/>
    ///
    /// For more details about SQLite cache modes:
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache"/>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache-size"/>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache-shared"/>
    /// </remarks>
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

    /// <summary>
    /// Converts to MySQL connection string format.
    /// </summary>
    /// <returns>A formatted MySQL connection string.</returns>
    /// <remarks>
    /// For more details about MySQL connection strings, see:
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
    /// For more details about SQL Server connection strings, see:
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
    /// Converts to MongoDB connection string format.
    /// </summary>
    /// <returns>A formatted MongoDB connection string.</returns>
    /// <remarks>
    /// For more details about MongoDB connection strings, see:
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
    /// For more details about Cassandra connection strings, see:
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
    /// For more details about Couchbase connection strings, see:
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
