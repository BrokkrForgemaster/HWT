using HWT.Domain.Entities;

namespace HWT.Application.Interfaces
{
    public interface IUexCorpService
    {
      
        Task<IReadOnlyList<CommodityFuture>> GetCommodityFuturesAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<CommodityPrice>> GetCommodityPricesByNameAsync(string commodityName, CancellationToken cancellationToken);
    }
}