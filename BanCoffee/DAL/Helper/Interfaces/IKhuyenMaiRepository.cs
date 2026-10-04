using Model;
using System.Collections.Generic;

namespace DAL
{
    public interface IKhuyenMaiRepository : ICreate<KhuyenMaiModel, bool>, IUpdate<KhuyenMaiModel, bool>, IDelete<string, bool>
    {
        KhuyenMaiModel GetDatabyID(string id);
        List<KhuyenMaiModel> GetAll();
        KhuyenMaiModel GetByCode(string code);
        List<KhuyenMaiModel> Search(int pageIndex, int pageSize, out long total, string ten, string trang_thai);
    }
}
