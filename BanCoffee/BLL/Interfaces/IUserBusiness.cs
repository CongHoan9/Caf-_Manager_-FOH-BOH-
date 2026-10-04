using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IUserBusiness
    {
        UserModel Authenticate(string username, string password);
        UserModel GetDatabyID(string id);
        List<UserModel> GetAll();
        bool Create(UserModel model);
        bool Update(UserModel model);
        bool Delete(string id);
        bool ChangePassword(string userId, string oldPassword, string newPassword);
        bool CheckExists(string taikhoan);
        List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan);
    }
}
