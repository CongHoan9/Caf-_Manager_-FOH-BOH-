using Model;
using System.Collections.Generic;

namespace BLL
{
    public partial interface IItemBusiness : ICreate<ItemModel, bool>, IUpdate<ItemModel, bool>, IDelete<string, bool>
    {
        ItemModel GetDatabyID(string id);
        List<ItemModel> GetDataAll();
        List<ItemModel> Search(int pageIndex, int pageSize, out long total, string item_group_id, string item_name);
    }
}
