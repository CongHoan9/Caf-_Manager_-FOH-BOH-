using DAL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public abstract class Repository(IDatabaseHelper dbHelper)
    {
        protected readonly IDatabaseHelper _dbHelper = dbHelper;

        public static T Execute<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ExecuteTransaction(string spName, params object[] parameters) => Execute(() =>
        {
            var result = _dbHelper.ExecuteScalarSProcedureWithTransaction(out string msgError, spName, parameters);
            if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
            {
                throw new Exception(Convert.ToString(result) + msgError);
            }
            return true;
        });
        public List<T> ExecuteQuery<T>(string spName, params object[] parameters) where T : new() => Execute(() =>
        {
            var dt = _dbHelper.ExecuteSProcedureReturnDataTable(out string msgError, spName, parameters);
            if (!string.IsNullOrEmpty(msgError))
            {
                throw new Exception(msgError);
            }
            return dt != null ? dt.ConvertTo<T>().ToList() : new List<T>();
        });
    }
}
