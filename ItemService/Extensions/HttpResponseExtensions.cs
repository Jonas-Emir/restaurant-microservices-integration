using System.Text.Json;
using ItemService.Dtos;
using Microsoft.AspNetCore.Http;

namespace ItemService.Extensions;

public static class HttpResponseExtensions
{
    public static void AddPaginationHeader<T>(this HttpResponse response, PagedResult<T> pagedResult)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(pagedResult);

        var metadata = new
        {
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage
        };

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        response.Headers.Append("Access-Control-Expose-Headers", "X-Pagination");
    }
}
