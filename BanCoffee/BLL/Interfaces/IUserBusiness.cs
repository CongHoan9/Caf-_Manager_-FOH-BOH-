using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IUserBusiness
    {
        UserModel Authenticate(string username, string password);
        UserModel GetDatabyID(string id);
        bool Create(UserModel model);
        bool Update(UserModel model);
        bool Delete(string id);
        List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan);
    }
}
