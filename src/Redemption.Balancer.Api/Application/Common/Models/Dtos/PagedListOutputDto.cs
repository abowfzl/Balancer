using System.Collections;

namespace Redemption.Balancer.Api.Application.Common.Models.Dtos;

[Serializable]
public class PagedListOutputDto
{
    public PagedListOutputDto(IEnumerable items, int pageNumber, int pageSize, int totalCount)
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
        TotalCount = totalCount;
        Items = items;
    }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public IEnumerable Items { get; set; }
}