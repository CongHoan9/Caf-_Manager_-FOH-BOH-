#pragma warning disable IDE1006
using System;

namespace Model
{
    /// <summary>Lich lam viec (bang [lich_lam_viec]) - phan ca nhan vien.</summary>
    public class LichLamViecModel {
        public string lich_id { get; set; }
        public string user_id { get; set; }
        public string ca_lam_id { get; set; }
        public DateTime ngay_lam { get; set; }
        public string ghi_chu { get; set; }
        // Joined fields for display
        public string hoten { get; set; }
        public string ten_ca { get; set; }
    }
}


