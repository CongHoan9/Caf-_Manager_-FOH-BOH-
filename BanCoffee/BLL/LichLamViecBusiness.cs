using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class LichLamViecBusiness(ILichLamViecRepository res) : Business<ILichLamViecRepository>(res), ILichLamViecBusiness
    {
        public bool Create(LichLamViecModel model)
        {
            model.lich_id = Guid.NewGuid().ToString();
            return _res.Create(model);
        }
        public bool Update(LichLamViecModel model) => _res.Update(model);
        public bool Delete(string id) => _res.Delete(id);
        public LichLamViecModel GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<LichLamViecModel> Search(int pageIndex, int pageSize, out long total, string user_id, DateTime? ngay_lam, string ca_lam_id) => _res.Search(pageIndex, pageSize, out total, user_id, ngay_lam, ca_lam_id);
    }
}



