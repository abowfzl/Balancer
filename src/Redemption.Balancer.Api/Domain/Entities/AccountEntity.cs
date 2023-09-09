namespace Redemption.Balancer.Api.Domain.Entities;

public class AccountEntity : BaseEntity
{
    public string? Name { get; set; }

    public int StemeraldUserId { get; set; }

    public virtual ICollection<AccountConfigEntity> AccountConfigs { get; set; } = new List<AccountConfigEntity>();

    public virtual ICollection<TransactionEntity> FromTransactions { get; set; } = new List<TransactionEntity>();

    public virtual ICollection<TransactionEntity> ToTransactions { get; set; } = new List<TransactionEntity>();
}