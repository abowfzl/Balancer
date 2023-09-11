namespace Redemption.Balancer.Api.Infrastructure.Common.Extensions;

public static class PriceExtensions
{
    public static decimal CalculateSymbolPrice(decimal baseNormalizationScale, decimal quoteNormalizationScale, decimal baseCurrencyTicker, decimal quoteCurrencyTicker)
    {
        var normalizationScaleDifference = CalculateNormalizationScaleDifference(baseNormalizationScale, quoteNormalizationScale);

        var demoralizedRate = CalculateDenormalizedRate(baseCurrencyTicker, quoteCurrencyTicker);

        var price = demoralizedRate * (decimal)Math.Pow(10, (double)normalizationScaleDifference);

        return price;
    }

    private static decimal CalculateNormalizationScaleDifference(decimal baseNormalizationScale, decimal quoteNormalizationScale)
    {
        return baseNormalizationScale - quoteNormalizationScale;
    }

    private static decimal CalculateDenormalizedRate(decimal baseTicker, decimal quoteTicker)
    {
        return baseTicker / quoteTicker;
    }

    public static decimal Denormalize(decimal number, int currencyNormalizationScale)
    {
        var normalizedNumber = number * (decimal)Math.Pow(10, currencyNormalizationScale);

        return normalizedNumber;
    }

    public static decimal Normalize(decimal number, int currencyNormalizationScale)
    {
        var normalizedNumber = number * (decimal)Math.Pow(10, -currencyNormalizationScale);

        return normalizedNumber;
    }
}
