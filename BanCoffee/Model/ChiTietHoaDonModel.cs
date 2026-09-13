namespace Model
{
    /// <summary>
    /// Dong mon trong hoa don (bang [chi_tiet_hoa_don]) - detail.
    /// Khi Update: status 1 = them moi, 2 = sua, 3 = xoa.
    /// </summary>
    public class ChiTietHoaDonModel
    {
        public string Ma_chi_tiet { get; set; }
        public string Ma_hoa_don { get; set; }
        public string Item_id { get; set; }
        public string Item_name { get; set; }   // hien thi, khong co cot SQL
        public int So_luong { get; set; }
        public double Don_gia { get; set; }    // gia chot tai thoi diem ban
        public int Status { get; set; }
    }
}
