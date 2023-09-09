namespace Redemption.Balancer.Api.Application.Common.Contracts;

public interface IDateTimeProvider
{
    DateTime SetKindUtc(DateTime dateTime);
}