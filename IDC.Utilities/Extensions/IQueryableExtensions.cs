using System.Linq.Expressions;
using IDC.Utilities.Models.Data;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides helper methods for handling pagination operations.
/// </summary>
/// <remarks>
/// Contains extension methods to facilitate pagination operations on IQueryable data sources.
/// Supports dynamic sorting and filtering based on Bootstrap-Table parameters.
/// </remarks>
public static class IQueryableExtensions
{
    /// <summary>
    /// Applies pagination parameters to an IQueryable source.
    /// </summary>
    /// <typeparam name="T">The type of data being paginated.</typeparam>
    /// <param name="source">The IQueryable data source.</param>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>A PaginationResponse containing the paginated data.</returns>
    /// <remarks>
    /// Handles sorting, filtering, and pagination based on Bootstrap-Table parameters.
    /// </remarks>
    /// <example>
    /// <code>
    /// var query = dbContext.Users.AsQueryable();
    /// var request = new PaginationRequest { Offset = 0, Limit = 10 };
    /// var response = await query.ToPaginatedResponseAsync(request: request);
    /// </code>
    /// </example>
    public static async Task<PaginationResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> source,
        PaginationRequest request
    )
    {
        var totalNotFiltered = await Task.FromResult(source.Count());
        var total = totalNotFiltered;
        var rows = await Task.FromResult(source.Skip(request.Offset).Take(request.Limit).ToList());

        return new PaginationResponse<T>
        {
            Total = total,
            TotalNotFiltered = totalNotFiltered,
            Rows = rows
        };
    }

    /// <summary>
    /// Applies pagination parameters to an IQueryable source with custom sorting.
    /// </summary>
    /// <typeparam name="T">The type of data being paginated.</typeparam>
    /// <param name="source">The IQueryable data source.</param>
    /// <param name="request">The pagination request parameters.</param>
    /// <param name="defaultSort">Expression for default sorting when no sort parameter is provided.</param>
    /// <returns>A PaginationResponse containing the paginated and sorted data.</returns>
    /// <remarks>
    /// Supports custom default sorting when no sort parameter is specified in the request.
    /// </remarks>
    /// <example>
    /// <code>
    /// var query = dbContext.Users.AsQueryable();
    /// var request = new PaginationRequest { Offset = 0, Limit = 10 };
    /// var response = await query.ToPaginatedResponseAsync(
    ///     request: request,
    ///     defaultSort: x => x.Id
    /// );
    /// </code>
    /// </example>
    public static async Task<PaginationResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> source,
        PaginationRequest request,
        Expression<Func<T, object>> defaultSort
    )
    {
        var sortedQuery = string.IsNullOrEmpty(request.Sort) ? source.OrderBy(defaultSort) : source;

        return await sortedQuery.ToPaginatedResponseAsync(request: request);
    }
}
