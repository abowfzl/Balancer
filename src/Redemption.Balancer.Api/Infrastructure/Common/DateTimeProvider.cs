using Redemption.Balancer.Api.Application.Common.Contracts;

namespace Redemption.Balancer.Api.Infrastructure.Common;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime SetKindUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}