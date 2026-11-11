using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IItemGroupBusiness
    {
        List<ItemGroupModel> GetData();
    }
}
