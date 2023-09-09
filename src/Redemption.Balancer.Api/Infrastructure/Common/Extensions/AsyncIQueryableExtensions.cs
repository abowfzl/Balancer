using Microsoft.EntityFrameworkCore;
using Redemption.Balancer.Api.Application.Common.Models.Dtos;

namespace Redemption.Balancer.Api.Infrastructure.Common.Extensions;

public static class AsyncIQueryableExtensions
{
    public static async Task<PagedListOutputDto> ToPagedListAsync<T>(this IQueryable<T>? source, int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        if (source == null)
            return new PagedListOutputDto(null!, pageNumber, pageSize, 0);

        var count = await source.CountAsync(cancellationToken);

        pageSize = pageSize <= 0 ? count : pageSize;

        var data = new List<T>();

        // e.g:for page number 1 we should skip 0 item
        var pageIndex = pageNumber - 1;

        // Min allowed page index is 0
        pageIndex = Math.Max(pageIndex, 0);

        data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken));

        return new PagedListOutputDto(data, Math.Max(pageIndex, 1), pageSize, count);
    }
}