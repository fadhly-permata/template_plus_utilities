namespace IDC.Utilities.Models.Data;

/// <summary>
/// Represents pagination request parameters compatible with Bootstrap-Table.
/// </summary>
/// <remarks>
/// Provides standard pagination parameters used by Bootstrap-Table for server-side pagination.
/// Parameters align with Bootstrap-Table's client-side pagination configuration.
/// </remarks>
/// <example>
/// <code>
/// var request = new PaginationRequest
/// {
///     Offset = 0,
///     Limit = 10,
///     Sort = "id",
///     Order = "asc",
///     Search = "john"
/// };
/// </code>
/// </example>
public class PaginationRequest
{
    /// <summary>Gets or sets the number of records to skip.</summary>
    /// <value>The offset value for pagination.</value>
    public int Offset { get; set; }

    /// <summary>Gets or sets the number of records per page.</summary>
    /// <value>The page size limit.</value>
    public int Limit { get; set; } = 10;

    /// <summary>Gets or sets the field name to sort by.</summary>
    /// <value>The sort column name.</value>
    public string? Sort { get; set; }

    /// <summary>Gets or sets the sort direction (asc/desc).</summary>
    /// <value>The sort order direction.</value>
    public string? Order { get; set; }

    /// <summary>Gets or sets the search query string.</summary>
    /// <value>The search term for filtering.</value>
    public string? Search { get; set; }
}
