#pragma warning disable IDE1006
namespace Model
{
    public class CongThucModel
    {
        public string cong_thuc_id { get; set; }
        public string item_id { get; set; }
        public string nguyen_lieu_id { get; set; }
        public double so_luong_tieu_hao { get; set; }
        
        // Mo rong de hien thi / Validate
        public string ten_nguyen_lieu { get; set; }
        public double so_luong_ton { get; set; }
    }
}
