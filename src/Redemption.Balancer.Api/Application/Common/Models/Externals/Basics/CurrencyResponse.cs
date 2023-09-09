namespace Redemption.Balancer.Api.Application.Common.Models.Externals.Basics;

public class CurrencyResponse
{
    public string? Symbol { get; set; }

    public string? Name { get; set; }

    public int NormalizationScale { get; set; }

    public int SmallestUnitScale { get; set; }

    public string? SwapStep { get; set; }

    public string? Type { get; set; }
}
