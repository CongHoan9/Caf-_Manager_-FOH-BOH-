using Model;
using System.Collections.Generic;

namespace DAL
{
    public interface IBanRepository : ICreate<BanModel, bool>, IUpdate<BanModel, bool>, IDelete<string, bool>
    {
        BanModel GetDatabyID(string id);
        List<BanModel> GetAll();
        bool UpdateTrangThai(string ban_id, string trang_thai);
        List<BanModel> Search(int pageIndex, int pageSize, out long total, string ten_ban);
    }
}
