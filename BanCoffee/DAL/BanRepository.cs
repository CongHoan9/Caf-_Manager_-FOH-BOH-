using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL
{
    public partial class BanRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), IBanRepository
    {
        public bool Create(BanModel model) => ExecuteTransaction("sp_ban_create",
            "@ban_id", model.ban_id,
            "@ten_ban", model.ten_ban,
            "@khu_vuc", model.khu_vuc,
            "@so_cho", model.so_cho,
            "@trang_thai", model.trang_thai);

        public bool Update(BanModel model) => ExecuteTransaction("sp_ban_update",
            "@ban_id", model.ban_id,
            "@ten_ban", model.ten_ban,
            "@khu_vuc", model.khu_vuc,
            "@so_cho", model.so_cho,
            "@trang_thai", model.trang_thai);

        public bool Delete(string id) => ExecuteTransaction("sp_ban_delete", "@ban_id", id);

        public BanModel GetDatabyID(string id) => ExecuteQuery<BanModel>("sp_ban_get_by_id", "@ban_id", id).FirstOrDefault();

        public List<BanModel> GetAll() => ExecuteQuery<BanModel>("sp_ban_all");

        public bool UpdateTrangThai(string ban_id, string trang_thai) => ExecuteTransaction("sp_ban_update_trang_thai", "@ban_id", ban_id, "@trang_thai", trang_thai);

        public List<BanModel> Search(int pageIndex, int pageSize, out long total, string ten_ban)
        {
            return ExecuteSearch<BanModel>(out total, "sp_ban_search",
                "@page_index", pageIndex,
                "@page_size", pageSize,
                "@ten_ban", ten_ban);
        }
    }
}




















