namespace IDC.Utilities.Models.Data;

/// <summary>
/// Represents pagination request parameters compatible with Bootstrap-Table's server-side pagination.
/// </summary>
/// <remarks>
/// Provides a standardized structure for handling client-side pagination requests from Bootstrap-Table.
/// This class maps directly to Bootstrap-Table's client-side pagination configuration parameters.
///
/// Example request URL:
/// ```
/// GET /api/users?offset=0&amp;limit=10&amp;sort=name&amp;order=asc&amp;search=john
/// ```
///
/// Common usage scenarios:
/// - REST API pagination endpoints
/// - Data grid implementations
/// - Server-side filtering and sorting
///
/// > [!NOTE]
/// > All properties are designed to match Bootstrap-Table's query parameter names
/// > for seamless integration with its server-side pagination feature.
///
/// > [!TIP]
/// > Consider implementing request validation to ensure reasonable limit values
/// > and valid sort field names.
///
/// > [!IMPORTANT]
/// > The `Limit` property has a default value of 10 to prevent unlimited data fetching.
/// </remarks>
/// <seealso href="https://bootstrap-table.com/docs/api/table-options/#server-side-pagination">Bootstrap-Table Server-side Pagination</seealso>
public class PaginationRequest
{
    /// <summary>
    /// Gets or sets the number of records to skip for pagination.
    /// </summary>
    /// <value>
    /// An integer representing the offset from the start of the dataset.
    /// Used in conjunction with <see cref="Limit"/> for pagination calculations.
    /// </value>
    /// <remarks>
    /// This value is used to calculate the starting point for data retrieval.
    ///
    /// Example calculation:
    /// ```csharp
    /// var pageNumber = (Offset / Limit) + 1;
    /// var skipCount = Offset;
    /// ```
    ///
    /// > [!NOTE]
    /// > The offset is zero-based, meaning the first page starts at offset 0.
    /// </remarks>
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of records to return per page.
    /// </summary>
    /// <value>
    /// An integer representing the page size, defaulting to 10.
    /// </value>
    /// <remarks>
    /// Controls the number of records returned in each response.
    ///
    /// Example usage:
    /// ```csharp
    /// var pagedData = query
    ///     .Skip(request.Offset)
    ///     .Take(request.Limit)
    ///     .ToList();
    /// ```
    ///
    /// > [!IMPORTANT]
    /// > Consider implementing a maximum limit to prevent performance issues
    /// > with very large page sizes.
    /// </remarks>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Gets or sets the field name to sort the results by.
    /// </summary>
    /// <value>
    /// A string representing the sort column name.
    /// Can be null when no sorting is requested.
    /// </value>
    /// <remarks>
    /// Specifies which field should be used for ordering the results.
    /// Should correspond to a valid property name in your data model.
    ///
    /// Example usage:
    /// ```csharp
    /// if (!string.IsNullOrEmpty(request.Sort))
    /// {
    ///     query = request.Order?.ToLower() == "desc"
    ///         ? query.OrderByDescending(request.Sort)
    ///         : query.OrderBy(request.Sort);
    /// }
    /// ```
    ///
    /// > [!WARNING]
    /// > Validate sort field names to prevent SQL injection and errors.
    /// </remarks>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the sort direction for the results.
    /// </summary>
    /// <value>
    /// A string indicating sort direction ("asc" or "desc").
    /// Can be null when no specific order is requested.
    /// </value>
    /// <remarks>
    /// Determines whether to sort in ascending or descending order.
    /// Used in conjunction with <see cref="Sort"/>.
    ///
    /// Expected values:
    /// - "asc" for ascending order
    /// - "desc" for descending order
    ///
    /// > [!TIP]
    /// > Consider implementing a case-insensitive comparison for order values.
    /// </remarks>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets the search query for filtering results.
    /// </summary>
    /// <value>
    /// A string containing the search terms.
    /// Can be null when no search is performed.
    /// </value>
    /// <remarks>
    /// Used for implementing global search functionality across relevant fields.
    ///
    /// Example implementation:
    /// ```csharp
    /// if (!string.IsNullOrEmpty(request.Search))
    /// {
    ///     query = query.Where(x =>
    ///         x.Name.Contains(request.Search) ||
    ///         x.Description.Contains(request.Search)
    ///     );
    /// }
    /// ```
    ///
    /// > [!IMPORTANT]
    /// > Consider implementing proper search sanitization and optimization
    /// > for large datasets.
    /// </remarks>
    public string? Search { get; set; }
}
