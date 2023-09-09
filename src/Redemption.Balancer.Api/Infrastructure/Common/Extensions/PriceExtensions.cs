namespace Redemption.Balancer.Api.Infrastructure.Common.Extensions;

public class PriceExtensions
{
    public static decimal CalculateDenormalizedPrice(decimal basePrice, decimal quotePrice)
    {
        return basePrice / quotePrice;
    }
}
