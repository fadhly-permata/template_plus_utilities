namespace IDC.Utilities.Models.Data;

/// <summary>
/// Represents pagination response data compatible with Bootstrap-Table.
/// </summary>
/// <typeparam name="T">The type of data being paginated.</typeparam>
/// <remarks>
/// Provides a standardized response format for Bootstrap-Table server-side pagination.
/// Includes total count, total not filtered, and rows of data matching Bootstrap-Table's expected response format.
/// </remarks>
/// <example>
/// <code>
/// var response = new PaginationResponse&lt;UserModel&gt;
/// {
///     Total = 50,
///     TotalNotFiltered = 100,
///     Rows = usersList
/// };
/// </code>
/// </example>
public class PaginationResponse<T>
{
    /// <summary>Gets or sets the total number of filtered records.</summary>
    /// <value>The total count of records after applying filters.</value>
    public int Total { get; set; }

    /// <summary>Gets or sets the total number of records before filtering.</summary>
    /// <value>The total count of all records before applying any filters.</value>
    public int TotalNotFiltered { get; set; }

    /// <summary>Gets or sets the current page of data.</summary>
    /// <value>The list of records for the current page.</value>
    public List<T>? Rows { get; set; } = [];
}
