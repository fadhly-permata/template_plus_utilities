using IDC.Template.Utilities;
using IDC.Template.Utilities.Models.SQLite;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Controllers;

public partial class DemoController
{
    /// <summary>
    /// Creates a new SQLite table with specified columns
    /// </summary>
    /// <remarks>
    /// Creates a table in SQLite database with the given name and column definitions.
    /// Each column is defined by a key-value pair where key is the column name and value is the column type/definition.
    ///
    /// Example request body:
    /// {
    ///   "tableName": "users",
    ///   "columns": {
    ///     "id": "INTEGER PRIMARY KEY",
    ///     "name": "TEXT NOT NULL",
    ///     "email": "TEXT UNIQUE"
    ///   }
    /// }
    /// </remarks>
    /// <param name="request">Request object containing table name and column definitions</param>
    /// <returns>APIResponseData containing affected rows count</returns>W
    /// <exception cref="Exception">Thrown when table creation fails</exception>
    [Tags(tags: "SQLite In Memory"), HttpPost(template: "SQLite/Tables")]
    public APIResponseData<object> CreateTable([FromBody] CreateTableRequest request)
    {
        try
        {
            sqliteHelper!
                .Connect()
                .ExecuteNonQuery(
                    query: $"CREATE TABLE IF NOT EXISTS {request.TableName} ({(string.Join(separator: ", ", values: request.Columns.Select(selector: col => $"{col.Key} {col.Value}")))})",
                    affectedRows: out int affectedRows
                );

            return new APIResponseData<object>()
                .ChangeMessage(language: language, key: "api.ddl.table_created")
                .ChangeData(data: new { affectedRows });
        }
        catch (Exception ex)
        {
            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Alters a SQLite table by adding or removing columns
    /// </summary>
    /// <remarks>
    /// Modifies an existing table by adding new columns or removing existing ones.
    ///
    /// Example request body:
    /// {
    ///   "columnsToAdd": {
    ///     "age": "INTEGER",
    ///     "address": "TEXT"
    ///   },
    ///   "columnsToRemove": [
    ///     "old_column"
    ///   ]
    /// }
    /// </remarks>
    /// <param name="tableName">Name of the table to alter</param>
    /// <param name="request">Request object containing columns to add and remove</param>
    /// <returns>APIResponseData containing affected rows count</returns>
    /// <exception cref="Exception">Thrown when table alteration fails</exception>
    [Tags(tags: "SQLite In Memory"), HttpPut(template: "SQLite/Tables/{tableName}")]
    public APIResponseData<object> AlterTable(
        [FromRoute] string tableName,
        [FromBody] AlterTableRequest request
    )
    {
        try
        {
            var queries = new List<string>();

            if ((request.ColumnsToAdd?.Count ?? 0) > 0)
            {
                queries.AddRange(
                    (request.ColumnsToAdd ?? []).Select(selector: col =>
                        $"ALTER TABLE {tableName} ADD COLUMN {col.Key} {col.Value}"
                    )
                );
            }

            if ((request.ColumnsToRemove?.Count ?? 0) > 0)
            {
                queries.AddRange(
                    (request.ColumnsToRemove ?? []).Select(selector: col =>
                        $"ALTER TABLE {tableName} DROP COLUMN {col}"
                    )
                );
            }

            if (queries.Count > 0)
            {
                sqliteHelper!
                    .Connect()
                    .ExecuteNonQuery(
                        query: string.Join(separator: "; ", values: queries),
                        affectedRows: out int affectedRows
                    );

                return new APIResponseData<object>()
                    .ChangeMessage(language: language, key: "api.ddl.table_altered")
                    .ChangeData(data: new { affectedRows });
            }

            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(language: language, key: "api.ddl.no_columns_specified");
        }
        catch (Exception ex)
        {
            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Drops a SQLite table if it exists
    /// </summary>
    /// <remarks>
    /// Removes the specified table from the SQLite database.
    /// Uses IF EXISTS clause to prevent errors if table doesn't exist.
    /// Returns affected rows count in the response data.
    /// </remarks>
    /// <param name="tableName">Name of the table to drop</param>
    /// <returns>APIResponseData containing affected rows count</returns>
    /// <exception cref="Exception">Thrown when table drop operation fails</exception>
    [Tags(tags: "SQLite In Memory"), HttpDelete(template: "SQLite/Tables/{tableName}")]
    public APIResponseData<object> DropTable([FromRoute] string tableName)
    {
        try
        {
            sqliteHelper!
                .Connect()
                .ExecuteNonQuery(
                    query: $"DROP TABLE IF EXISTS {tableName}",
                    affectedRows: out int affectedRows
                );

            return new APIResponseData<object>()
                .ChangeMessage(language: language, key: "api.ddl.table_dropped")
                .ChangeData(data: new { affectedRows });
        }
        catch (Exception ex)
        {
            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets all tables in the SQLite database
    /// </summary>
    /// <remarks>
    /// Retrieves a list of all user tables in the database along with their column definitions.
    /// System tables (starting with 'sqlite_') are excluded from the results.
    ///
    /// Example response:
    /// {
    ///   "data": [
    ///     {
    ///       "tableName": "users",
    ///       "columns": [
    ///         {
    ///           "name": "id",
    ///           "type": "INTEGER",
    ///           "notNull": true,
    ///           "defaultValue": null,
    ///           "isPrimaryKey": true
    ///         }
    ///       ]
    ///     }
    ///   ]
    /// }
    /// </remarks>
    /// <returns>APIResponseData containing list of tables with their column definitions</returns>
    /// <exception cref="Exception">Thrown when table information cannot be retrieved</exception>
    [Tags(tags: "SQLite In Memory"), HttpGet(template: "SQLite/Tables")]
    public APIResponseData<object> GetAllTables()
    {
        try
        {
            sqliteHelper!
                .Connect()
                .ExecuteQuery(
                    query: "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'",
                    results: out List<JObject> tables
                );

            return new APIResponseData<object>()
                .ChangeMessage(language: language, key: "api.ddl.table_retrieved")
                .ChangeData(
                    data: tables
                        .Select(selector: table =>
                        {
                            var tableName = table["name"]?.ToString();
                            sqliteHelper.ExecuteQuery(
                                query: $"PRAGMA table_info({tableName})",
                                results: out List<JObject> columnData
                            );

                            return new
                            {
                                tableName,
                                columns = columnData.Select(selector: col => new
                                {
                                    name = col["name"],
                                    type = col["type"],
                                    notNull = col["notnull"],
                                    defaultValue = col["dflt_value"],
                                    isPrimaryKey = col["pk"]
                                })
                            };
                        })
                        .ToList()
                );
        }
        catch (Exception ex)
        {
            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets detailed information about a specific table
    /// </summary>
    /// <remarks>
    /// Retrieves column definitions including name, type, constraints and other metadata for the specified table
    /// </remarks>
    /// <param name="tableName">Name of the table to get information for</param>
    /// <returns>Table metadata including column definitions</returns>
    /// <exception cref="Exception">Thrown when table information cannot be retrieved</exception>
    [Tags(tags: "SQLite In Memory"), HttpGet(template: "SQLite/Tables/{tableName}")]
    public APIResponseData<object> GetTableInfo([FromRoute] string tableName)
    {
        try
        {
            sqliteHelper!
                .Connect()
                .ExecuteQuery(
                    query: $"PRAGMA table_info({tableName})",
                    results: out List<JObject> columnData
                );

            return new APIResponseData<object>()
                .ChangeMessage(language: language, key: "api.ddl.table_info_retrieved")
                .ChangeData(
                    data: new
                    {
                        tableName,
                        columns = columnData.Select(selector: col => new
                        {
                            name = col["name"],
                            type = col["type"],
                            notNull = col["notnull"],
                            defaultValue = col["dflt_value"],
                            isPrimaryKey = col["pk"]
                        })
                    }
                );
        }
        catch (Exception ex)
        {
            return new APIResponseData<object>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
