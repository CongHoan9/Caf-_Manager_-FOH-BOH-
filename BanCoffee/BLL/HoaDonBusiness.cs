using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class HoaDonBusiness(IHoaDonRepository res) : Business<IHoaDonRepository>(res), IHoaDonBusiness
    {
        public bool Create(HoaDonModel model)
        {
            model.ma_hoa_don = Guid.NewGuid().ToString();
            if (model.listjson_chitiet != null)
            {
                foreach (var item in model.listjson_chitiet)
                {
                    item.ma_hoa_don = model.ma_hoa_don;
                    item.ma_chi_tiet = Guid.NewGuid().ToString();
                }

            }
            return _res.Create(model);
        }
        public bool Update(HoaDonModel model)
        {
            if (model.listjson_chitiet != null)
            {
                foreach (var item in model.listjson_chitiet)
                    if (item.status == 1)
                    {
                        item.ma_hoa_don = model.ma_hoa_don;
                        item.ma_chi_tiet = Guid.NewGuid().ToString();
                    }
            }
            return _res.Update(model);
        }
        public bool Delete(string id)
        {
            return _res.Delete(id);
        }
        public HoaDonModel GetDatabyID(string id)
        {
            return _res.GetDatabyID(id);
        }
        public List<HoaDonModel> Search(int pageIndex, int pageSize, out long total, string hoten, string diachi)
        {
            return _res.Search(pageIndex, pageSize, out total, hoten, diachi);
        }
    }

}
