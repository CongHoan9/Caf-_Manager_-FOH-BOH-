using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class HoaDonBusiness(IHoaDonRepository res, IKhuyenMaiRepository khuyenMaiRes = null) : Business<IHoaDonRepository>(res), IHoaDonBusiness
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
                {
                    if (item.status == 1)
                    {
                        item.ma_hoa_don = model.ma_hoa_don;
                        item.ma_chi_tiet = Guid.NewGuid().ToString();
                    }
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
        public List<HoaDonModel> Search(int pageIndex, int pageSize, out long total, string hoten, string diachi, string user_id)
        {
            return _res.Search(pageIndex, pageSize, out total, hoten, diachi, user_id);
        }
        /// <summary>He thuc hien tinh: tong_tien = SUM(so_luong x don_gia); thanh_tien = tong_tien - giam_gia.</summary>
        private void TinhTien(HoaDonModel model)
        {
            double tong = 0;
            if (model.listjson_chitiet != null)
            {
                foreach (var item in model.listjson_chitiet)
                {
                    if (item.status == 3)
                    {
                        continue; // dong bi xoa khong tinh vao tong moi
                    } 
                    tong += item.so_luong * item.don_gia;
                }
            }
            model.tong_tien = tong;
            var giamGia = 0.0;
            if (!string.IsNullOrEmpty(model.khuyen_mai_id) && khuyenMaiRes != null)
            {
                var km = khuyenMaiRes.GetDatabyID(model.khuyen_mai_id);
                if (km != null && km.trang_thai == "Hoạt động" && km.ngay_bat_dau <= DateTime.Now && km.ngay_ket_thuc >= DateTime.Now)
                {
                    if (km.loai_giam == "Phần trăm") giamGia = tong * (km.gia_tri_giam / 100.0);
                    else giamGia = km.gia_tri_giam;
                }
            }
            giamGia = Math.Min(giamGia, tong);
            model.giam_gia = giamGia;
            model.thanh_tien = tong - giamGia;
        }
    }
}

