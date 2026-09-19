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
            TinhTien(model);
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
            TinhTien(model);
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

        /// <summary>He thuc hien tinh: tong_tien = SUM(so_luong x don_gia); thanh_tien = tong_tien - giam_gia.</summary>
        private static void TinhTien(HoaDonModel model)
        {
            double tong = 0;
            if (model.listjson_chitiet != null)
            {
                foreach (var item in model.listjson_chitiet)
                {
                    if (item.status == 3) continue; // dong bi xoa khong tinh vao tong moi
                    tong += item.so_luong * (item.don_gia ?? 0);
                }
            }
            model.tong_tien = tong;
            var giamGia = Math.Min(model.giam_gia ?? 0, tong);
            model.giam_gia = giamGia;
            model.thanh_tien = tong - giamGia;
        }
    }
}
