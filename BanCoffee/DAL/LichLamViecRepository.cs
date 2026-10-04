using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL
{
    public partial class LichLamViecRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), ILichLamViecRepository
    {
        public bool Create(LichLamViecModel model) => ExecuteTransaction("sp_lich_lam_viec_create",
            "@lich_id", model.lich_id,
            "@user_id", model.user_id,
            "@ca_lam_id", model.ca_lam_id,
            "@ngay_lam", model.ngay_lam,
            "@ghi_chu", model.ghi_chu);

        public bool Update(LichLamViecModel model) => ExecuteTransaction("sp_lich_lam_viec_update",
            "@lich_id", model.lich_id,
            "@user_id", model.user_id,
            "@ca_lam_id", model.ca_lam_id,
            "@ngay_lam", model.ngay_lam,
            "@ghi_chu", model.ghi_chu);

        public bool Delete(string id) => ExecuteTransaction("sp_lich_lam_viec_delete", "@lich_id", id);

        public LichLamViecModel GetDatabyID(string id) => ExecuteQuery<LichLamViecModel>("sp_lich_lam_viec_get_by_id", "@lich_id", id).FirstOrDefault();

        public List<LichLamViecModel> Search(int pageIndex, int pageSize, out long total, string user_id, DateTime? ngay_lam, string ca_lam_id)
        {
            return ExecuteSearch<LichLamViecModel>(out total, "sp_lich_lam_viec_search",
                "@page_index", pageIndex,
                "@page_size", pageSize,
                "@user_id", user_id,
                "@ngay_lam", ngay_lam,
                "@ca_lam_id", ca_lam_id);
        }
    }
}




































