using Model;
using System;
using System.Collections.Generic;

namespace DAL
{
    public interface ILichLamViecRepository : ICreate<LichLamViecModel, bool>, IUpdate<LichLamViecModel, bool>, IDelete<string, bool>
    {
        LichLamViecModel GetDatabyID(string id);
        List<LichLamViecModel> Search(int pageIndex, int pageSize, out long total, string user_id, DateTime? ngay_lam, string ca_lam_id);
    }
}
