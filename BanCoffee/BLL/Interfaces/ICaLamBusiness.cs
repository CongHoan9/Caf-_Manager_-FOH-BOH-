using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface ICaLamBusiness
    {
        bool Create(CaLamModel model);
        bool Update(CaLamModel model);
        bool Delete(string id);
        CaLamModel GetDatabyID(string id);
        List<CaLamModel> GetAll();
    }
}
