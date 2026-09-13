namespace Model
{
    /// <summary>Mon uong (bang [item]) - gia dung double? de khop kieu float cua SQL Server.</summary>
    public class ItemModel
    {
        public string Item_id { get; set; }
        public string Item_group_id { get; set; }
        public string Item_name { get; set; }
        public string Item_image { get; set; }
        public double Item_price { get; set; }
    }
}
