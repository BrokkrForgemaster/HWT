namespace HWT.Domain.Entities
{
    public abstract class RefineryItem
    {
        public int Id { get; set; }
        public int IdCommodity { get; set; }
        public int Quantity { get; set; }         // units
        public int Yield { get; set; }            // units
        public int YieldBonus { get; set; }       // percent
        public string CommodityName { get; set; }
        public string CommodityCode { get; set; }
        public string CommoditySlug { get; set; }
    }
}