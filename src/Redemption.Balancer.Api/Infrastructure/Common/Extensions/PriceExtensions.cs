namespace Redemption.Balancer.Api.Infrastructure.Common.Extensions;

public static class PriceExtensions
{
    public static decimal CalculateDenormalizedPrice(decimal basePrice, decimal quotePrice)
    {
        return basePrice / quotePrice;
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
