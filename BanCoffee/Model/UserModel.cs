#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>Nhan vien / quan tri (bang [user]) - dang nhap he thong BanCoffee.</summary>
    public class UserModel {
        public string user_id { get; set; }
        public string hoten { get; set; }
        public DateTime ngaysinh { get; set; }
        public string diachi { get; set; }
        public string gioitinh { get; set; }
        public string email { get; set; }
        public string taikhoan { get; set; }
        public string matkhau { get; set; }
        public string role { get; set; }      // Role.Admin / Role.Staff
        public string token { get; set; }     // JWT sinh tai login, khong co cot SQL
        public string image_url { get; set; }
    }
}


