using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Hoa don (bang [hoa_don]) - master cua cap master-detail 1-n voi ChiTietHoaDonModel.
    /// Mo rong cua BanCoffee: ngay_tao, tong_tien, giam_gia, thanh_tien, hinh_thuc_thanh_toan, ghi_chu, user_id.
    /// </summary>
    public class HoaDonModel
    {
        public string Ma_hoa_don { get; set; }
        public string Ho_ten { get; set; }
        public string Dia_chi { get; set; }
        public DateTime Ngay_tao { get; set; }
        public double Tong_tien { get; set; }
        public double Giam_gia { get; set; }
        public double Thanh_tien { get; set; }
        public string Hinh_thuc_thanh_toan { get; set; }      // PaymentMethod.*
        public string Ghi_chu { get; set; }
        public string User_id { get; set; }                   // nhan vien thanh toan
        public List<ChiTietHoaDonModel> Listjson_chitiet { get; set; }
    }
}
