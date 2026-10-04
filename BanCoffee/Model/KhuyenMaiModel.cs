#pragma warning disable IDE1006
using System;

namespace Model
{
    /// <summary>Khuyen mai / ma giam gia (bang [khuyen_mai]).</summary>
    public class KhuyenMaiModel {
        public string khuyen_mai_id { get; set; }
        public string ma_khuyen_mai { get; set; }
        public string ten_khuyen_mai { get; set; }
        public string loai_giam { get; set; }
        public double gia_tri_giam { get; set; }
        public DateTime ngay_bat_dau { get; set; }
        public DateTime ngay_ket_thuc { get; set; }
        public string trang_thai { get; set; }
    }
}


