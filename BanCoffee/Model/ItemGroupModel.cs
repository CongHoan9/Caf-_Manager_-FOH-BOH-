using System.Collections.Generic;

namespace Model
{
    /// <summary>Nhom do uong (bang [item_group]) - ho tro cay phan cap qua children.</summary>
    public class ItemGroupModel
    {
        public string Parent_item_group_id { get; set; }
        public string Item_group_id { get; set; }
        public string Item_group_name { get; set; }
        public string Url { get; set; }
        public short? Seq_num { get; set; }
        public List<ItemGroupModel> Children { get; set; }   // BLL dung cay, khong co cot SQL
        public string Type { get; set; }                     // "leaf" neu khong co con
    }
}
