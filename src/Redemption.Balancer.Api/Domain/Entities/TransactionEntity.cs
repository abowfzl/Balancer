namespace Redemption.Balancer.Api.Domain.Entities;

public class TransactionEntity : BaseEntity
{
    public int FromAccountId { get; set; }
    public virtual AccountEntity? FromAccount { get; set; }
    public int ToAccountId { get; set; }
    public virtual AccountEntity? ToAccount { get; set; }
    public string? Symbol { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalValue { get; set; }
}