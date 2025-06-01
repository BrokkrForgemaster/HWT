// HWT.Infrastructure/DTOs/CommodityAveragesDto.cs
namespace HWT.Infrastructure.DTOs
{
    public class CommodityAveragesDto
    {
        public int    Id               { get; set; }
            public int    IdCommodity      { get; set; }

            // BUY side averages
            public float  PriceBuy         { get; set; }
            public float  PriceBuyMin      { get; set; }
            public float  PriceBuyMinWeek  { get; set; }
            public float  PriceBuyMinMonth { get; set; }
            public float  PriceBuyMax      { get; set; }
            public float  PriceBuyMaxWeek  { get; set; }
            public float  PriceBuyMaxMonth { get; set; }
            public float  PriceBuyAvg      { get; set; }
            public float  PriceBuyAvgWeek  { get; set; }
            public float  PriceBuyAvgMonth { get; set; }
            public float  PriceBuyUsers    { get; set; }
            public int?   PriceBuyUsersRows{ get; set; }

            // SELL side averages
            public float  PriceSell         { get; set; }
            public float  PriceSellMin      { get; set; }
            public float  PriceSellMinWeek  { get; set; }
            public float  PriceSellMinMonth { get; set; }
            public float  PriceSellMax      { get; set; }
            public float  PriceSellMaxWeek  { get; set; }
            public float  PriceSellMaxMonth { get; set; }
            public float  PriceSellAvg      { get; set; }
            public float  PriceSellAvgWeek  { get; set; }
            public float  PriceSellUsers    { get; set; }
            public int?   PriceSellUsersRows{ get; set; }

            // SCU BUY side
            public float  ScuBuy            { get; set; }
            public float  ScuBuyMin         { get; set; }
            public float  ScuBuyMinWeek     { get; set; }
            public float  ScuBuyMinMonth    { get; set; }
            public float  ScuBuyMax         { get; set; }
            public float  ScuBuyMaxWeek     { get; set; }
            public float  ScuBuyMaxMonth    { get; set; }
            public float  ScuBuyAvg         { get; set; }
            public float  ScuBuyAvgWeek     { get; set; }
            public float  ScuBuyAvgMonth    { get; set; }
            public float  ScuBuyTotal       { get; set; }
            public float  ScuBuyTotalWeek   { get; set; }
            public float  ScuBuyTotalMonth  { get; set; }
            public float  ScuBuyUsers       { get; set; }
            public int?   ScuBuyUsersRows   { get; set; }

            // SCU SELL side
            public float  ScuSellStock      { get; set; }
            public float  ScuSellStockWeek  { get; set; }
            public float  ScuSellStockMonth { get; set; }
            public float  ScuSell           { get; set; }
            public float  ScuSellMin        { get; set; }
            public float  ScuSellMinWeek    { get; set; }
            public float  ScuSellMinMonth   { get; set; }
            public float  ScuSellMax        { get; set; }
            public float  ScuSellMaxWeek    { get; set; }
            public float  ScuSellMaxMonth   { get; set; }
            public float  ScuSellAvg        { get; set; }
            public float  ScuSellAvgWeek    { get; set; }
            public float  ScuSellAvgMonth   { get; set; }
            public float  ScuSellTotal      { get; set; }
            public float  ScuSellTotalWeek  { get; set; }
            public float  ScuSellTotalMonth { get; set; }
            public float  ScuSellUsers      { get; set; }
            public int?   ScuSellUsersRows  { get; set; }

            // INVENTORY BUY
            public int?   StatusBuy          { get; set; }
            public int?   StatusBuyMin       { get; set; }
            public int?   StatusBuyMinWeek   { get; set; }
            public int?   StatusBuyMinMonth  { get; set; }
            public int?   StatusBuyMax       { get; set; }
            public int?   StatusBuyMaxWeek   { get; set; }
            public int?   StatusBuyMaxMonth  { get; set; }
            public int?   StatusBuyAvg       { get; set; }
            public int?   StatusBuyAvgWeek   { get; set; }
            public int?   StatusBuyAvgMonth  { get; set; }

            // INVENTORY SELL
            public int?   StatusSell         { get; set; }
            public int?   StatusSellMin      { get; set; }
            public int?   StatusSellMinWeek  { get; set; }
            public int?   StatusSellMinMonth { get; set; }
            public int?   StatusSellMax      { get; set; }
            public int?   StatusSellMaxWeek  { get; set; }
            public int?   StatusSellMaxMonth { get; set; }
            public int?   StatusSellAvg      { get; set; }
            public int?   StatusSellAvgWeek  { get; set; }
            public int?   StatusSellAvgMonth { get; set; }

            // VOLATILITY
            public float  VolatilityPriceBuy    { get; set; }
            public float  VolatilityPriceSell   { get; set; }
            public float  VolatilityScuBuy      { get; set; }
            public float  VolatilityScuSell     { get; set; }


            // etc
            public int    CaxScore             { get; set; }
            public string GameVersion          { get; set; } = string.Empty;
            public long   DateAdded            { get; set; }
            public long   DateModified         { get; set; }
            public string CommodityName        { get; set; } = string.Empty;
            public string CommodityCode        { get; set; } = string.Empty;
            public string CommoditySlug        { get; set; } = string.Empty;
    }
}
