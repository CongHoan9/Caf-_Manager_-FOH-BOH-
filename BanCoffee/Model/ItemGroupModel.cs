#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Model
{
    /// <summary>Nhom do uong (bang [item_group]) - ho tro cay phan cap qua children.</summary>
    public class ItemGroupModel {
        public string parent_item_group_id { get; set; }
        public string item_group_id { get; set; }
        public string item_group_name { get; set; }
        public string url { get; set; }
        public short seq_num { get; set; }
        public List<ItemGroupModel> children { get; set; }   // BLL dung cay, khong co cot SQL
        public string type { get; set; }                     // "leaf" neu khong co con
    }
}


