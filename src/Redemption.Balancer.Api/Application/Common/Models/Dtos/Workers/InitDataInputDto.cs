namespace Redemption.Balancer.Api.Application.Common.Models.Dtos.Workers;

public class InitDataInputDto
{
    public IList<InitDataDetail> Data { get; set; } = new List<InitDataDetail>();
}

public class InitDataDetail
{
    public string Name { get; set; }

    public int StemeraldUserId { get; set; }

    public IList<InitDataConfigs> Configs { get; set; } = new List<InitDataConfigs>();
}

public class InitDataConfigs
{
    public string Symbol { get; set; }

    public decimal Value { get; set; }
}
