using DAL.Helper;
using Model;
using Helper;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAL
{
    public partial class CaLamRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), ICaLamRepository
    {
        public bool Create(CaLamModel model) => ExecuteTransaction("sp_ca_lam_create",
            "@ca_lam_id", model.ca_lam_id,
            "@ten_ca", model.ten_ca,
            "@gio_bat_dau", model.gio_bat_dau,
            "@gio_ket_thuc", model.gio_ket_thuc);

        public bool Update(CaLamModel model) => ExecuteTransaction("sp_ca_lam_update",
            "@ca_lam_id", model.ca_lam_id,
            "@ten_ca", model.ten_ca,
            "@gio_bat_dau", model.gio_bat_dau,
            "@gio_ket_thuc", model.gio_ket_thuc);

        public bool Delete(string id) => ExecuteTransaction("sp_ca_lam_delete", "@ca_lam_id", id);

        public CaLamModel GetDatabyID(string id) => ExecuteQuery<CaLamModel>("sp_ca_lam_get_by_id", "@ca_lam_id", id).FirstOrDefault();

        public List<CaLamModel> GetAll() => ExecuteQuery<CaLamModel>("sp_ca_lam_all");
    }
}
















