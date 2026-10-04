#pragma warning disable IDE1006
namespace Model
{
    /// <summary>Mon uong (bang [item]) - gia dung double? de khop kieu float cua SQL Server.</summary>
    public class ItemModel {
        public string item_id { get; set; }
        public string item_group_id { get; set; }
        public string item_name { get; set; }
        public string item_image { get; set; }
        public double item_price { get; set; }
    }
}


