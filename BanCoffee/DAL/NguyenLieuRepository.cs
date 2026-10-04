using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL
{
    public partial class NguyenLieuRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), INguyenLieuRepository
    {
        public bool Create(NguyenLieuModel model) => ExecuteTransaction("sp_nguyen_lieu_create",
            "@nguyen_lieu_id", model.nguyen_lieu_id,
            "@ten_nguyen_lieu", model.ten_nguyen_lieu,
            "@don_vi", model.don_vi,
            "@so_luong_ton", model.so_luong_ton,
            "@gia_nhap", model.gia_nhap,
            "@ngay_nhap_cuoi", model.ngay_nhap_cuoi);

        public bool Update(NguyenLieuModel model) => ExecuteTransaction("sp_nguyen_lieu_update",
            "@nguyen_lieu_id", model.nguyen_lieu_id,
            "@ten_nguyen_lieu", model.ten_nguyen_lieu,
            "@don_vi", model.don_vi,
            "@so_luong_ton", model.so_luong_ton,
            "@gia_nhap", model.gia_nhap,
            "@ngay_nhap_cuoi", model.ngay_nhap_cuoi);

        public bool Delete(string id) => ExecuteTransaction("sp_nguyen_lieu_delete", "@nguyen_lieu_id", id);

        public NguyenLieuModel GetDatabyID(string id) => ExecuteQuery<NguyenLieuModel>("sp_nguyen_lieu_get_by_id", "@nguyen_lieu_id", id).FirstOrDefault();

        public List<NguyenLieuModel> GetAll() => ExecuteQuery<NguyenLieuModel>("sp_nguyen_lieu_all");

        public bool NhapThem(string id, double soLuong, double giaNhap) => ExecuteTransaction("sp_nguyen_lieu_nhap_them",
            "@nguyen_lieu_id", id,
            "@so_luong_them", soLuong,
            "@gia_nhap_moi", giaNhap,
            "@ngay_nhap", DateTime.Now);

        public List<NguyenLieuModel> Search(int pageIndex, int pageSize, out long total, string ten_nguyen_lieu)
        {
            return ExecuteSearch<NguyenLieuModel>(out total, "sp_nguyen_lieu_search",
                "@page_index", pageIndex,
                "@page_size", pageSize,
                "@ten_nguyen_lieu", ten_nguyen_lieu);
        }
    }
}












































