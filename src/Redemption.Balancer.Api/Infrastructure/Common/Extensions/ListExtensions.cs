using Redemption.Balancer.Api.Application.Common.Models.Dtos;

namespace Redemption.Balancer.Api.Infrastructure.Common.Extensions;

public static class ListExtensions
{
    public static PagedListOutputDto ToPagedList<T>(this IList<T>? source, int pageNumber, int pageSize)
    {
        if (source == null)
            return new PagedListOutputDto(null!, pageNumber, pageSize, 0);

        var count = source.Count;

        pageSize = pageSize <= 0 ? count : pageSize;

        var data = new List<T>();

        // e.g:for page number 1 we should skip 0 item
        var pageIndex = pageNumber - 1;

        // Min allowed page index is 0
        pageIndex = Math.Max(pageIndex, 0);

        data.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize));

        return new PagedListOutputDto(data, Math.Max(pageIndex, 1), pageSize, count);
    }
}