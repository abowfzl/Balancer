namespace Redemption.Balancer.Api.Application.Common.Models;

public class BusinessDetailModel<T>
{
    public string Name { get; set; }

    public T Detail { get; set; }
}

public class TransactionBusinessModel
{
    public int Id { get; set; }
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public string? Symbol { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalValue { get; set; }
}
