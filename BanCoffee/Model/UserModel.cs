using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>Nhan vien / quan tri (bang [user]) - dang nhap he thong BanCoffee.</summary>
    public class UserModel
    {
        public string User_id { get; set; }
        public string Hoten { get; set; }
        public DateTime? Ngaysinh { get; set; }
        public string Diachi { get; set; }
        public string Gioitinh { get; set; }
        public string Email { get; set; }
        public string Taikhoan { get; set; }
        public string Matkhau { get; set; }
        public string Role { get; set; }      // Role.Admin / Role.Staff
        public string Token { get; set; }     // JWT sinh tai login, khong co cot SQL
        public string Image_url { get; set; }
    }
}
