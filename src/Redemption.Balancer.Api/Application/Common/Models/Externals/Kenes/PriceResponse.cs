namespace Redemption.Balancer.Api.Application.Common.Models.Externals.Kenes;

public class PriceResponse
{
    public DateTime CreatedAt { get; set; }

    public int Id { get; set; }

    public string CurrencySymbol { get; set; } = null!;

    public string CriterionCurrencySymbol { get; set; } = null!;

    public decimal Ticker { get; set; }

    public decimal Swing { get; set; }
}