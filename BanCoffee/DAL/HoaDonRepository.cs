using DAL.Helper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public partial class HoaDonRepository : Repository, IHoaDonRepository
    {
        public HoaDonRepository(IDatabaseHelper dbHelper) : base(dbHelper) { }
        public bool Create(HoaDonModel model) => ExecuteTransaction("sp_hoa_don_create",
                                                                    "@ma_hoa_don", model.ma_hoa_don,
                                                                    "@ho_ten", model.ho_ten,
                                                                    "@dia_chi", model.dia_chi,
                                                                    "@ngay_tao", model.ngay_tao,
                                                                    "@tong_tien", model.tong_tien,
                                                                    "@giam_gia", model.giam_gia,
                                                                    "@thanh_tien", model.thanh_tien,
                                                                    "@hinh_thuc_thanh_toan", model.hinh_thuc_thanh_toan,
                                                                    "@ghi_chu", model.ghi_chu,
                                                                    "@user_id", model.user_id,
                                                                    "@listjson_chitiet", model.listjson_chitiet != null ? MessageConvert.SerializeObject(model.listjson_chitiet) : null);
        public bool Update(HoaDonModel model) => ExecuteTransaction("sp_hoa_don_update",
                                                                    "@ma_hoa_don", model.ma_hoa_don,
                                                                    "@ho_ten", model.ho_ten,
                                                                    "@dia_chi", model.dia_chi,
                                                                    "@ngay_tao", model.ngay_tao,
                                                                    "@tong_tien", model.tong_tien,
                                                                    "@giam_gia", model.giam_gia,
                                                                    "@thanh_tien", model.thanh_tien,
                                                                    "@hinh_thuc_thanh_toan", model.hinh_thuc_thanh_toan,
                                                                    "@ghi_chu", model.ghi_chu,
                                                                    "@user_id", model.user_id,
                                                                    "@listjson_chitiet", model.listjson_chitiet != null ? MessageConvert.SerializeObject(model.listjson_chitiet) : null);
        public HoaDonModel GetDatabyID(string id) => ExecuteQuery<HoaDonModel>("sp_hoa_don_get_by_id", "@ma_hoa_don", id).FirstOrDefault();
        public bool Delete(string id) => ExecuteTransaction("sp_hoa_don_delete", "@ma_hoa_don", id);
        public List<HoaDonModel> Search(int pageIndex, int pageSize, out long total, string hoten, string diachi)
        {
            total = 0;
            return ExecuteQuery<HoaDonModel>("sp_hoa_don_search",
                                             "@page_index", pageIndex,
                                             "@page_size", pageSize,
                                             "@hoten", hoten,
                                             "@diachi", diachi);
        }
    }
}
