namespace IDC.Utilities.Models.Data;

/// <summary>
/// Represents a standardized pagination response data structure compatible with Bootstrap-Table.
/// </summary>
/// <typeparam name="T">The type of data being paginated.</typeparam>
/// <remarks>
/// Provides a standardized response format for server-side pagination that aligns with Bootstrap-Table's requirements.
/// This class encapsulates pagination metadata and the actual data rows.
///
/// Example response JSON:
/// ```json
/// {
///     "total": 50,
///     "totalNotFiltered": 100,
///     "rows": [
///         { "id": 1, "name": "John Doe" },
///         { "id": 2, "name": "Jane Smith" }
///     ]
/// }
/// ```
///
/// Common usage scenarios:
/// - REST API pagination responses
/// - Data grid implementations
/// - Server-side filtering and sorting
///
/// > [!NOTE]
/// > This class is specifically designed to work with Bootstrap-Table's
/// > server-side pagination feature. Ensure your client-side implementation
/// > matches Bootstrap-Table's expected format.
///
/// > [!TIP]
/// > For optimal performance, consider implementing database-level pagination
/// > when working with large datasets.
///
/// > [!IMPORTANT]
/// > The `Rows` property is initialized as an empty list by default to prevent null reference exceptions.
/// </remarks>
/// <seealso href="https://bootstrap-table.com/docs/api/table-options/#server-side-pagination">Bootstrap-Table Server-side Pagination</seealso>
public class PaginationResponse<T>
{
    /// <summary>
    /// Gets or sets the total number of records after applying filters.
    /// </summary>
    /// <value>
    /// An integer representing the count of filtered records.
    /// This value may be less than or equal to <see cref="TotalNotFiltered"/>.
    /// </value>
    /// <remarks>
    /// This property is used by Bootstrap-Table to:
    /// - Calculate the total number of pages
    /// - Display pagination information
    /// - Enable/disable pagination controls
    ///
    /// Example calculation:
    /// ```csharp
    /// Total = filteredData.Count();
    /// ```
    /// </remarks>
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the total number of records before applying any filters.
    /// </summary>
    /// <value>
    /// An integer representing the total count of all available records.
    /// This value represents the complete dataset size.
    /// </value>
    /// <remarks>
    /// Used to provide context about the total dataset size when filtering is applied.
    /// Helps users understand how many records were excluded by their filter criteria.
    ///
    /// Example calculation:
    /// ```csharp
    /// TotalNotFiltered = allData.Count();
    /// ```
    ///
    /// > [!NOTE]
    /// > This value should remain constant regardless of applied filters.
    /// </remarks>
    public int TotalNotFiltered { get; set; }

    /// <summary>
    /// Gets or sets the collection of records for the current page.
    /// </summary>
    /// <value>
    /// A list of type <typeparamref name="T"/> containing the records for the current page.
    /// Initialized as an empty list by default.
    /// </value>
    /// <remarks>
    /// Contains the actual data to be displayed in the current page.
    /// The number of items should typically match the requested page size.
    ///
    /// Example usage:
    /// <code>
    /// var response = new PaginationResponse&lt;UserModel&gt;
    /// {
    ///     Rows = await dbContext.Users
    ///         .Skip(pageSize * (pageNumber - 1))
    ///         .Take(pageSize)
    ///         .ToListAsync()
    /// };
    /// </code>
    ///
    /// > [!IMPORTANT]
    /// > When implementing paging logic:
    /// > - Ensure proper ordering is applied before paging
    /// > - Consider performance implications for large offsets
    /// > - Implement proper null checking
    /// </remarks>
    public List<T>? Rows { get; set; } = [];
}
