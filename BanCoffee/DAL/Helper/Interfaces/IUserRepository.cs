using Model;
using System.Collections.Generic;
namespace DAL
{
    public interface IUserRepository : ICreate<UserModel, bool>, IUpdate<UserModel, bool>, IDelete<string, bool>
    {
        UserModel GetUser(string username, string password);
        UserModel GetDatabyID(string id);
        List<UserModel> Search(int pageIndex, int pageSize, out long total, string hoten, string taikhoan);
    }
}
