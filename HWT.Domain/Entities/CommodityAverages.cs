// HWT.Domain/Entities/CommodityAverages.cs
namespace HWT.Domain.Entities
{
    public class CommodityAverages
    {
        public int Id { get; set; }
        public int IdCommodity { get; set; }

        // We only need the “avg” fields for now.
        public float PriceBuyAvg { get; set; }
        public float PriceSellAvg { get; set; }

        // If you want to show SCU or volatility later, add:
        // public float ScuBuyAvg { get; set; }
        // public float ScuSellAvg { get; set; }

        public int CaxScore { get; set; }
        public string CommodityName { get; set; } = string.Empty;
        public string CommodityCode { get; set; } = string.Empty;
        public string CommoditySlug { get; set; } = string.Empty;
    }
}