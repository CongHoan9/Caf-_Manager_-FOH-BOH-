using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class BanBusiness(IBanRepository res) : Business<IBanRepository>(res), IBanBusiness
    {
        public bool Create(BanModel model)
        {
            model.ban_id = Guid.NewGuid().ToString();
            return _res.Create(model);
        }
        public bool Update(BanModel model) => _res.Update(model);
        public bool Delete(string id) => _res.Delete(id);
        public BanModel GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<BanModel> GetAll() => _res.GetAll();
        public bool UpdateTrangThai(string ban_id, string trang_thai) => _res.UpdateTrangThai(ban_id, trang_thai);
        public List<BanModel> Search(int pageIndex, int pageSize, out long total, string filter) => _res.Search(pageIndex, pageSize, out total, filter);
    }
}


