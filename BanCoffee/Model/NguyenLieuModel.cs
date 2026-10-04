#pragma warning disable IDE1006
using System;

namespace Model
{
    /// <summary>Nguyen lieu / kho (bang [nguyen_lieu]) - quan ly ton kho.</summary>
    public class NguyenLieuModel {
        public string nguyen_lieu_id { get; set; }
        public string ten_nguyen_lieu { get; set; }
        public string don_vi { get; set; }
        public double so_luong_ton { get; set; }
        public double gia_nhap { get; set; }
        public DateTime ngay_nhap_cuoi { get; set; }
    }
}


