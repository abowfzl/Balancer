using Newtonsoft.Json;

namespace Redemption.Balancer.Api.Application.Common.Models.Externals.Stemeralds;

public class StemeraldPriceResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("currencySymbol")]
    public string CurrencySymbol { get; set; } = null!;

    [JsonProperty("criterionCurrencySymbol")]
    public string CriterionCurrencySymbol { get; set; } = null!;

    [JsonProperty("ticker")]
    public string Ticker { get; set; } = null!;

    public decimal DecimalTicker { get => decimal.Parse(Ticker); }

    [JsonProperty("swing")]
    public string Swing { get; set; } = null!;

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
}
