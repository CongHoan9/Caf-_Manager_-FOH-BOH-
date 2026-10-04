using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface INguyenLieuBusiness
    {
        bool Create(NguyenLieuModel model);
        bool Update(NguyenLieuModel model);
        bool Delete(string id);
        NguyenLieuModel GetDatabyID(string id);
        List<NguyenLieuModel> GetAll();
        bool NhapThem(string id, double soLuong, double giaNhap);
        List<NguyenLieuModel> Search(int pageIndex, int pageSize, out long total, string filter);
    }
}
