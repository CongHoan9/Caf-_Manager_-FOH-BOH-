using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class CaLamBusiness(ICaLamRepository res) : Business<ICaLamRepository>(res), ICaLamBusiness
    {
        public bool Create(CaLamModel model)
        {
            model.ca_lam_id = Guid.NewGuid().ToString();
            return _res.Create(model);
        }
        public bool Update(CaLamModel model) => _res.Update(model);
        public bool Delete(string id) => _res.Delete(id);
        public CaLamModel GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<CaLamModel> GetAll() => _res.GetAll();
    }
}


