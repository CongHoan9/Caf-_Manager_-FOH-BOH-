#pragma warning disable IDE1006
using System;

namespace Model
{
    /// <summary>Ca lam viec (bang [ca_lam]) - chia ca cho nhan vien.</summary>
    public class CaLamModel {
        public string ca_lam_id { get; set; }
        public string ten_ca { get; set; }
        public TimeSpan gio_bat_dau { get; set; }
        public TimeSpan gio_ket_thuc { get; set; }
    }
}


