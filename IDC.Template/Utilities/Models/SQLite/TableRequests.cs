namespace IDC.Template.Utilities.Models.SQLite;

/// <summary>
/// Request model for creating a new SQLite table
/// </summary>
/// <remarks>
/// Used to specify table name and column definitions when creating a new table.
/// Column names are dictionary keys, and their SQLite data types are the values.
///
/// Example:
/// {
///   "tableName": "users",
///   "columns": {
///     "id": "INTEGER PRIMARY KEY",
///     "name": "TEXT NOT NULL",
///     "email": "TEXT UNIQUE"
///   }
/// }
/// </remarks>
public class CreateTableRequest
{
    /// <summary>
    /// Name of the table to be created
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary of column definitions where key is column name and value is SQLite data type
    /// </summary>
    /// <remarks>
    /// Example:
    /// {
    ///   "id": "INTEGER PRIMARY KEY",
    ///   "name": "TEXT NOT NULL",
    ///   "email": "TEXT UNIQUE"
    /// }
    /// </remarks>
    public Dictionary<string, string> Columns { get; set; } = [];
}

/// <summary>
/// Request model for altering an existing SQLite table
/// </summary>
/// <remarks>
/// Supports adding new columns and removing existing ones.
/// For adding columns, specify column names and their SQLite data types.
/// For removing columns, provide just the column names to drop.
///
/// Example:
/// {
///   "columnsToAdd": {
///     "phone": "TEXT",
///     "address": "TEXT NULL"
///   },
///   "columnsToRemove": ["old_column", "unused_field"]
/// }
/// </remarks>
public class AlterTableRequest
{
    /// <summary>
    /// Dictionary of columns to add where key is column name and value is column type/definition
    /// </summary>
    /// <remarks>
    /// Example:
    /// {
    ///   "phone": "TEXT",
    ///   "address": "TEXT NULL"
    /// }
    /// </remarks>
    public Dictionary<string, string>? ColumnsToAdd { get; set; }

    /// <summary>
    /// List of column names to be removed from the table
    /// </summary>
    /// <remarks>
    /// Example:
    /// ["old_column", "unused_field"]
    /// </remarks>
    public List<string>? ColumnsToRemove { get; set; }
}
