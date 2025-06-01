using Newtonsoft.Json;

namespace HWT.Domain.Entities
{
    public class CommodityPrice
    {
        // Identifiers
        [JsonProperty("id")]                    
        public int    Id                    { get; set; }

        [JsonProperty("id_commodity")]           
        public int    IdCommodity           { get; set; }

        [JsonProperty("id_star_system")]          
        public int    IdStarSystem          { get; set; }

        [JsonProperty("id_planet")]              
        public int    IdPlanet              { get; set; }

        [JsonProperty("id_orbit")]               
        public int    IdOrbit               { get; set; }

        [JsonProperty("id_moon")]                
        public int    IdMoon                { get; set; }

        [JsonProperty("id_city")]                
        public int    IdCity                { get; set; }

        [JsonProperty("id_outpost")]             
        public int    IdOutpost             { get; set; }

        [JsonProperty("id_poi")]                 
        public int    IdPoi                 { get; set; }

        [JsonProperty("id_faction")]             
        public int    IdFaction             { get; set; }

        [JsonProperty("id_terminal")]            
        public int    IdTerminal            { get; set; }

        // (sometimes the API returns "id_faction" twice; if so, bind the duplicate into this nullable)
        [JsonProperty("id_faction_duplicate")]
        public int?   IdFactionDuplicate    { get; set; }

        // Buy prices (last / min / max / avg)
        [JsonProperty("price_buy")]
        public float  PriceBuy              { get; set; }

        [JsonProperty("price_buy_min")]
        public float  PriceBuyMin           { get; set; }

        [JsonProperty("price_buy_min_week")]
        public float  PriceBuyMinWeek       { get; set; }

        [JsonProperty("price_buy_min_month")]
        public float  PriceBuyMinMonth      { get; set; }

        [JsonProperty("price_buy_max")]
        public float  PriceBuyMax           { get; set; }

        [JsonProperty("price_buy_max_week")]
        public float  PriceBuyMaxWeek       { get; set; }

        [JsonProperty("price_buy_max_month")]
        public float  PriceBuyMaxMonth      { get; set; }

        [JsonProperty("price_buy_avg")]
        public float  PriceBuyAvg           { get; set; }

        [JsonProperty("price_buy_avg_week")]
        public float  PriceBuyAvgWeek       { get; set; }

        [JsonProperty("price_buy_avg_month")]
        public float  PriceBuyAvgMonth      { get; set; }

        [JsonProperty("price_buy_users")]
        public float  PriceBuyUsers         { get; set; }

        [JsonProperty("price_buy_users_rows")]
        public int?   PriceBuyUsersRows     { get; set; }

        // Sell prices (last / min / max / avg)
        [JsonProperty("price_sell")]
        public float  PriceSell             { get; set; }

        [JsonProperty("price_sell_min")]
        public float  PriceSellMin          { get; set; }

        [JsonProperty("price_sell_min_week")]
        public float  PriceSellMinWeek      { get; set; }

        [JsonProperty("price_sell_min_month")]
        public float  PriceSellMinMonth     { get; set; }

        [JsonProperty("price_sell_max")]
        public float  PriceSellMax          { get; set; }

        [JsonProperty("price_sell_max_week")]
        public float  PriceSellMaxWeek      { get; set; }

        [JsonProperty("price_sell_max_month")]
        public float  PriceSellMaxMonth     { get; set; }

        [JsonProperty("price_sell_avg")]
        public float  PriceSellAvg          { get; set; }

        [JsonProperty("price_sell_avg_week")]
        public float  PriceSellAvgWeek      { get; set; }

        [JsonProperty("price_sell_avg_month")]
        public float  PriceSellAvgMonth     { get; set; }

        [JsonProperty("price_sell_users")]
        public float  PriceSellUsers        { get; set; }

        [JsonProperty("price_sell_users_rows")]
        public int?   PriceSellUsersRows    { get; set; }

        // SCU buy (last / min / max / avg / total / users)
        [JsonProperty("scu_buy")]
        public float  ScuBuy                { get; set; }

        [JsonProperty("scu_buy_min")]
        public float  ScuBuyMin             { get; set; }

        [JsonProperty("scu_buy_min_week")]
        public float  ScuBuyMinWeek         { get; set; }

        [JsonProperty("scu_buy_min_month")]
        public float  ScuBuyMinMonth        { get; set; }

        [JsonProperty("scu_buy_max")]
        public float  ScuBuyMax             { get; set; }

        [JsonProperty("scu_buy_max_week")]
        public float  ScuBuyMaxWeek         { get; set; }

        [JsonProperty("scu_buy_max_month")]
        public float  ScuBuyMaxMonth        { get; set; }

        [JsonProperty("scu_buy_avg")]
        public float  ScuBuyAvg             { get; set; }

        [JsonProperty("scu_buy_avg_week")]
        public float  ScuBuyAvgWeek         { get; set; }

        [JsonProperty("scu_buy_avg_month")]
        public float  ScuBuyAvgMonth        { get; set; }

        [JsonProperty("scu_buy_total")]
        public float  ScuBuyTotal           { get; set; }

        [JsonProperty("scu_buy_total_week")]
        public float  ScuBuyTotalWeek       { get; set; }

        [JsonProperty("scu_buy_total_month")]
        public float  ScuBuyTotalMonth      { get; set; }

        [JsonProperty("scu_buy_users")]
        public float  ScuBuyUsers           { get; set; }

        [JsonProperty("scu_buy_users_rows")]
        public int?   ScuBuyUsersRows       { get; set; }

        // Reported inventory SCU at location (sell)
        [JsonProperty("scu_sell_stock")]
        public float  ScuSellStock          { get; set; }

        [JsonProperty("scu_sell_stock_avg")]
        public float  ScuSellStockAvg       { get; set; }

        [JsonProperty("scu_sell_stock_avg_week")]
        public float  ScuSellStockAvgWeek   { get; set; }

        [JsonProperty("scu_sell_stock_avg_month")]
        public float  ScuSellStockAvgMonth  { get; set; }

        // SCU sell demand (last / min / max / avg / users)
        [JsonProperty("scu_sell")]
        public float  ScuSell               { get; set; }

        [JsonProperty("scu_sell_min")]
        public float  ScuSellMin            { get; set; }

        [JsonProperty("scu_sell_min_week")]
        public float  ScuSellMinWeek        { get; set; }

        [JsonProperty("scu_sell_min_month")]
        public float  ScuSellMinMonth       { get; set; }

        [JsonProperty("scu_sell_max")]
        public float  ScuSellMax            { get; set; }

        [JsonProperty("scu_sell_max_week")]
        public float  ScuSellMaxWeek        { get; set; }

        [JsonProperty("scu_sell_max_month")]
        public float  ScuSellMaxMonth       { get; set; }

        [JsonProperty("scu_sell_avg")]
        public float  ScuSellAvg            { get; set; }

        [JsonProperty("scu_sell_avg_week")]
        public float  ScuSellAvgWeek        { get; set; }

        [JsonProperty("scu_sell_avg_month")]
        public float  ScuSellAvgMonth       { get; set; }

        [JsonProperty("scu_sell_users")]
        public float  ScuSellUsers          { get; set; }

        [JsonProperty("scu_sell_users_rows")]
        public int?   ScuSellUsersRows      { get; set; }

        // Inventory state for purchase
        [JsonProperty("status_buy")]
        public int?   StatusBuy             { get; set; }

        [JsonProperty("status_buy_min")]
        public int?   StatusBuyMin          { get; set; }

        [JsonProperty("status_buy_min_week")]
        public int?   StatusBuyMinWeek      { get; set; }

        [JsonProperty("status_buy_min_month")]
        public int?   StatusBuyMinMonth     { get; set; }

        [JsonProperty("status_buy_max")]
        public int?   StatusBuyMax          { get; set; }

        [JsonProperty("status_buy_max_week")]
        public int?   StatusBuyMaxWeek      { get; set; }

        [JsonProperty("status_buy_max_month")]
        public int?   StatusBuyMaxMonth     { get; set; }

        [JsonProperty("status_buy_avg")]
        public int?   StatusBuyAvg          { get; set; }

        [JsonProperty("status_buy_avg_week")]
        public int?   StatusBuyAvgWeek      { get; set; }

        [JsonProperty("status_buy_avg_month")]
        public int?   StatusBuyAvgMonth     { get; set; }

        // Inventory state for sell
        [JsonProperty("status_sell")]
        public int?   StatusSell            { get; set; }

        [JsonProperty("status_sell_min")]
        public int?   StatusSellMin         { get; set; }

        [JsonProperty("status_sell_min_week")]
        public int?   StatusSellMinWeek     { get; set; }

        [JsonProperty("status_sell_min_month")]
        public int?   StatusSellMinMonth    { get; set; }

        [JsonProperty("status_sell_max")]
        public int?   StatusSellMax         { get; set; }

        [JsonProperty("status_sell_max_week")]
        public int?   StatusSellMaxWeek     { get; set; }

        [JsonProperty("status_sell_max_month")]
        public int?   StatusSellMaxMonth    { get; set; }

        [JsonProperty("status_sell_avg")]
        public int?   StatusSellAvg         { get; set; }

        [JsonProperty("status_sell_avg_week")]
        public int?   StatusSellAvgWeek     { get; set; }

        [JsonProperty("status_sell_avg_month")]
        public int?   StatusSellAvgMonth    { get; set; }

        // Volatility coefficients
        [JsonProperty("volatility_price_buy")]
        public float  VolatilityPriceBuy    { get; set; }

        [JsonProperty("volatility_price_sell")]
        public float  VolatilityPriceSell   { get; set; }

        [JsonProperty("volatility_scu_buy")]
        public float  VolatilityScuBuy      { get; set; }

        [JsonProperty("volatility_scu_sell")]
        public float  VolatilityScuSell     { get; set; }

        // Faction affinity & container sizes
        [JsonProperty("faction_affinity")]
        public int?   FactionAffinity       { get; set; }

        [JsonProperty("container_sizes")]
        public string? ContainerSizes       { get; set; }

        // Miscellaneous fields
        [JsonProperty("game_version")]
        public string  GameVersion           { get; set; } = string.Empty;

        [JsonProperty("date_added")]
        public int     DateAdded             { get; set; } // timestamp

        [JsonProperty("date_modified")]
        public int     DateModified          { get; set; } // timestamp

        // Commodity metadata
        [JsonProperty("commodity_name")]
        public string  CommodityName         { get; set; } = string.Empty;

        [JsonProperty("commodity_code")]
        public string  CommodityCode         { get; set; } = string.Empty;

        [JsonProperty("commodity_slug")]
        public string  CommoditySlug         { get; set; } = string.Empty;

        // Location names (nullable)
        [JsonProperty("star_system_name")]
        public string? StarSystemName       { get; set; }

        [JsonProperty("planet_name")]
        public string? PlanetName           { get; set; }

        [JsonProperty("orbit_name")]
        public string? OrbitName            { get; set; }

        [JsonProperty("moon_name")]
        public string? MoonName             { get; set; }

        [JsonProperty("space_station_name")]
        public string? SpaceStationName     { get; set; }

        [JsonProperty("outpost_name")]
        public string? OutpostName          { get; set; }

        [JsonProperty("city_name")]
        public string? CityName             { get; set; }

        [JsonProperty("poi_name")]
        public string? PoiName              { get; set; }

        [JsonProperty("faction_name")]
        public string? FactionName          { get; set; }

        // Terminal info (strings)
        [JsonProperty("terminal_name")]
        public string  TerminalName         { get; set; } = string.Empty;

        [JsonProperty("terminal_code")]
        public string  TerminalCode         { get; set; } = string.Empty;

        [JsonProperty("terminal_slug")]
        public string  TerminalSlug         { get; set; } = string.Empty;

        [JsonProperty("terminal_is_player_owned")]
        public int     TerminalIsPlayerOwned{ get; set; }
    }
}
