namespace Redemption.Balancer.Api.Domain.Entities;

public class AccountConfigEntity : BaseEntity
{
    public int AccountId { get; set; }
    public virtual AccountEntity? Account { get; set; }
    public string? Symbol { get; set; }
    public decimal Value { get; set; }
}