namespace Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

public class PagedListResponse<T>
{
    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public IList<T> Items { get; set; }
}
