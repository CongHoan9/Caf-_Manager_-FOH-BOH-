using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL
{
    public partial class KhuyenMaiRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), IKhuyenMaiRepository
    {
        public bool Create(KhuyenMaiModel model) => ExecuteTransaction("sp_khuyen_mai_create",
            "@khuyen_mai_id", model.khuyen_mai_id,
            "@ma_khuyen_mai", model.ma_khuyen_mai,
            "@ten_khuyen_mai", model.ten_khuyen_mai,
            "@loai_giam", model.loai_giam,
            "@gia_tri_giam", model.gia_tri_giam,
            "@ngay_bat_dau", model.ngay_bat_dau,
            "@ngay_ket_thuc", model.ngay_ket_thuc,
            "@trang_thai", model.trang_thai);

        public bool Update(KhuyenMaiModel model) => ExecuteTransaction("sp_khuyen_mai_update",
            "@khuyen_mai_id", model.khuyen_mai_id,
            "@ma_khuyen_mai", model.ma_khuyen_mai,
            "@ten_khuyen_mai", model.ten_khuyen_mai,
            "@loai_giam", model.loai_giam,
            "@gia_tri_giam", model.gia_tri_giam,
            "@ngay_bat_dau", model.ngay_bat_dau,
            "@ngay_ket_thuc", model.ngay_ket_thuc,
            "@trang_thai", model.trang_thai);

        public bool Delete(string id) => ExecuteTransaction("sp_khuyen_mai_delete", "@khuyen_mai_id", id);

        public KhuyenMaiModel GetDatabyID(string id) => ExecuteQuery<KhuyenMaiModel>("sp_khuyen_mai_get_by_id", "@khuyen_mai_id", id).FirstOrDefault();

        public List<KhuyenMaiModel> GetAll() => ExecuteQuery<KhuyenMaiModel>("sp_khuyen_mai_all");

        public KhuyenMaiModel GetByCode(string code) => ExecuteQuery<KhuyenMaiModel>("sp_khuyen_mai_get_by_code", "@ma_khuyen_mai", code).FirstOrDefault();

        public List<KhuyenMaiModel> Search(int pageIndex, int pageSize, out long total, string ten, string trang_thai)
        {
            return ExecuteSearch<KhuyenMaiModel>(out total, "sp_khuyen_mai_search",
                "@page_index", pageIndex,
                "@page_size", pageSize,
                "@ten_khuyen_mai", ten,
                "@trang_thai", trang_thai);
        }
    }
}








































