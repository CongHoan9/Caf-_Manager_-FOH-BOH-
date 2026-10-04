using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class NguyenLieuBusiness(INguyenLieuRepository res) : Business<INguyenLieuRepository>(res), INguyenLieuBusiness
    {
        private static readonly DateTime SqlMinDate = new(1753, 1, 1);

        public bool Create(NguyenLieuModel model)
        {
            Validate(model);
            model.nguyen_lieu_id = Guid.NewGuid().ToString();
            if (model.ngay_nhap_cuoi < SqlMinDate) model.ngay_nhap_cuoi = DateTime.Now;
            return _res.Create(model);
        }

        public bool Update(NguyenLieuModel model)
        {
            Validate(model);
            var existing = _res.GetDatabyID(model.nguyen_lieu_id) ?? throw new Exception("Nguyên liệu không tồn tại");
            // Giữ ngày nhập cuối cũ nếu frontend không gửi
            if (model.ngay_nhap_cuoi < SqlMinDate) model.ngay_nhap_cuoi = existing.ngay_nhap_cuoi;
            return _res.Update(model);
        }

        public bool Delete(string id) => _res.Delete(id);
        public NguyenLieuModel GetDatabyID(string id) => _res.GetDatabyID(id);
        public List<NguyenLieuModel> GetAll() => _res.GetAll();

        public bool NhapThem(string id, double soLuong, double giaNhap)
        {
            if (soLuong <= 0) throw new Exception("Số lượng nhập phải lớn hơn 0");
            var existing = _res.GetDatabyID(id) ?? throw new Exception("Nguyên liệu không tồn tại");
            // Không nhập giá mới -> giữ giá nhập hiện tại
            if (giaNhap <= 0) giaNhap = existing.gia_nhap;
            return _res.NhapThem(id, soLuong, giaNhap);
        }

        public List<NguyenLieuModel> Search(int pageIndex, int pageSize, out long total, string filter) => _res.Search(pageIndex, pageSize, out total, filter);

        private static void Validate(NguyenLieuModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ten_nguyen_lieu)) throw new Exception("Tên nguyên liệu không được để trống");
            if (string.IsNullOrWhiteSpace(model.don_vi)) throw new Exception("Đơn vị không được để trống");
            if (model.so_luong_ton < 0) throw new Exception("Số lượng tồn không được âm");
            if (model.gia_nhap < 0) throw new Exception("Giá nhập không được âm");
        }
    }
}
