using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IBanBusiness
    {
        bool Create(BanModel model);
        bool Update(BanModel model);
        bool Delete(string id);
        BanModel GetDatabyID(string id);
        List<BanModel> GetAll();
        bool UpdateTrangThai(string ban_id, string trang_thai);
        List<BanModel> Search(int pageIndex, int pageSize, out long total, string filter);
    }
}
