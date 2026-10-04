using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IKhuyenMaiBusiness
    {
        bool Create(KhuyenMaiModel model);
        bool Update(KhuyenMaiModel model);
        bool Delete(string id);
        KhuyenMaiModel GetDatabyID(string id);
        List<KhuyenMaiModel> GetAll();
        KhuyenMaiModel GetByCode(string code);
        List<KhuyenMaiModel> Search(int pageIndex, int pageSize, out long total, string ten, string trang_thai);
    }
}
