using Model;
using System.Collections.Generic;
namespace DAL
{
    public partial interface IHoaDonRepository : ICreate<HoaDonModel, bool>, IUpdate<HoaDonModel, bool>, IDelete<string, bool>
    {
        HoaDonModel GetDatabyID(string id);
        List<HoaDonModel> Search(int pageIndex, int pageSize, out long total, string hoten, string diachi);
    }
}
