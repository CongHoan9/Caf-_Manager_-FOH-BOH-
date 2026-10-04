using Model;
using System.Collections.Generic;

namespace DAL
{
    public interface INguyenLieuRepository : ICreate<NguyenLieuModel, bool>, IUpdate<NguyenLieuModel, bool>, IDelete<string, bool>
    {
        NguyenLieuModel GetDatabyID(string id);
        List<NguyenLieuModel> GetAll();
        bool NhapThem(string id, double soLuong, double giaNhap);
        List<NguyenLieuModel> Search(int pageIndex, int pageSize, out long total, string ten_nguyen_lieu);
    }
}
