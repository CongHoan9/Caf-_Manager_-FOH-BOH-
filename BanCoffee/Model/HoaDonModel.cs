#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Hoa don (bang [hoa_don]) - master cua cap master-detail 1-n voi ChiTietHoaDonModel.
    /// Mo rong cua BanCoffee: ngay_tao, tong_tien, giam_gia, thanh_tien, hinh_thuc_thanh_toan, ghi_chu, user_id.
    /// </summary>
    public class HoaDonModel {
        public string ma_hoa_don { get; set; }
        public string ho_ten { get; set; }
        public string dia_chi { get; set; }
        public DateTime ngay_tao { get; set; }
        public double tong_tien { get; set; }
        public double giam_gia { get; set; }
        public double thanh_tien { get; set; }
        public string hinh_thuc_thanh_toan { get; set; }     
        public string ghi_chu { get; set; }
        public string user_id { get; set; }                  
        public string ban_id { get; set; }
        public string khuyen_mai_id { get; set; }
        public List<ChiTietHoaDonModel> listjson_chitiet { get; set; }
    }
}



