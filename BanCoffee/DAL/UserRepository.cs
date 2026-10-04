using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace DAL
{
    public partial class UserRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), IUserRepository
    {
        public bool Create(UserModel model) => ExecuteTransaction("sp_user_create",
                                                                  "@user_id", model.user_id,
                                                                  "@hoten", model.hoten,
                                                                  "@ngaysinh", model.ngaysinh,
                                                                  "@diachi", model.diachi,
                                                                  "@gioitinh", model.gioitinh,
                                                                  "@email", model.email,
                                                                  "@taikhoan", model.taikhoan,
                                                                  "@matkhau", model.matkhau,
                                                                  "@role", model.role,
                                                                  "@image_url", model.image_url);

        public bool Delete(string id) => ExecuteTransaction("sp_user_delete", "@user_id", id);
        public bool Update(UserModel model) => ExecuteTransaction("sp_user_update",
                                                                  "@user_id", model.user_id,
                                                                  "@hoten", model.hoten,
                                                                  "@ngaysinh", model.ngaysinh,
                                                                  "@diachi", model.diachi,
                                                                  "@gioitinh", model.gioitinh,
                                                                  "@email", model.email,
                                                                  "@taikhoan", model.taikhoan,
                                                                  "@matkhau", model.matkhau,
                                                                  "@role", model.role,
                                                                  "@image_url", model.image_url);
        public UserModel GetUser(string username, string password) => ExecuteQuery<UserModel>("sp_user_get_by_username_password", "@taikhoan", username, "@matkhau", password).FirstOrDefault();
        public UserModel GetDatabyID(string id) => ExecuteQuery<UserModel>("sp_user_get_by_id", "@user_id", id).FirstOrDefault();
        public List<UserModel> GetAll() => ExecuteQuery<UserModel>("sp_user_all");
        public bool ChangePassword(string userId, string oldPassword, string newPassword) => ExecuteTransaction("sp_user_change_password",
                                                                  "@user_id", userId,
                                                                  "@old_password", oldPassword,
                                                                  "@new_password", newPassword);
        public bool CheckExists(string taikhoan) => ExecuteQuery<UserModel>("sp_user_check_exists", "@taikhoan", taikhoan).Any();
        public List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan)
        {
            return ExecuteSearch<UserModel>(out total, "sp_user_search",
                                           "@page_index", pageIndex,
                                           "@page_size", pageSize,
                                           "@hoten", hoten,
                                           "@taikhoan", taikhoan);
        }
    }
}































































