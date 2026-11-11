using Model;
using System;
using System.Collections.Generic;
using System.Text;
using DAL;
namespace BLL
{
    public partial interface IHoaDonBusiness : ICreate<HoaDonModel, bool>, IUpdate<HoaDonModel, bool>, IDelete<string, bool>
    {
        HoaDonModel GetDatabyID(string id);
        List<HoaDonModel> Search(int pageIndex, int pageSize, out long total, string hoten, string diachi);
    }
}
