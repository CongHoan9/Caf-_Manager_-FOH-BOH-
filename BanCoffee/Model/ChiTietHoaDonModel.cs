#pragma warning disable IDE1006
namespace Model
{
    /// <summary>
    /// Dong mon trong hoa don (bang [chi_tiet_hoa_don]) - detail.
    /// Khi Update: status 1 = them moi, 2 = sua, 3 = xoa.
    /// </summary>
    public class ChiTietHoaDonModel {
        public string ma_chi_tiet { get; set; }
        public string ma_hoa_don { get; set; }
        public string item_id { get; set; }
        public string item_name { get; set; }  
        public int so_luong { get; set; }
        public double don_gia { get; set; }   
        public int status { get; set; }
    }
}


