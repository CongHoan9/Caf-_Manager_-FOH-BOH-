using Model;
using System.Collections.Generic;

namespace DAL
{
    public interface ICaLamRepository : ICreate<CaLamModel, bool>, IUpdate<CaLamModel, bool>, IDelete<string, bool>
    {
        CaLamModel GetDatabyID(string id);
        List<CaLamModel> GetAll();
    }
}
