using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial interface ILichLamViecBusiness
    {
        bool Create(LichLamViecModel model);
        bool Update(LichLamViecModel model);
        bool Delete(string id);
        LichLamViecModel GetDatabyID(string id);
        List<LichLamViecModel> Search(int pageIndex, int pageSize, out long total, string user_id, DateTime? ngay_lam, string ca_lam_id);
    }
}

