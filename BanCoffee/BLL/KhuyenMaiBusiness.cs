using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class KhuyenMaiBusiness(IKhuyenMaiRepository res) : Business<IKhuyenMaiRepository>(res), IKhuyenMaiBusiness
    {
        public bool Create(KhuyenMaiModel model)
        {
            model.khuyen_mai_id = Guid.NewGuid().ToString();
            return _res.Create(model);
        }
        public bool Update(KhuyenMaiModel model) => _res.Update(model);
        public bool Delete(string id) => _res.Delete(id);
        public KhuyenMaiModel GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<KhuyenMaiModel> GetAll() => _res.GetAll();
        public KhuyenMaiModel GetByCode(string code) => _res.GetByCode(code);
        public List<KhuyenMaiModel> Search(int pageIndex, int pageSize, out long total, string ten, string trang_thai) => _res.Search(pageIndex, pageSize, out total, ten, trang_thai);
    }
}


