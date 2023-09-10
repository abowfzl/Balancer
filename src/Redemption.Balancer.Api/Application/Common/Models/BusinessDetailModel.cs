namespace Redemption.Balancer.Api.Application.Common.Models
{
    public class BusinessDetailModel<T>
    {
        public string Name { get; set; }

        public T Detail { get; set; }
    }
}
