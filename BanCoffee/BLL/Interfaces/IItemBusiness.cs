using Model;
using System;
using System.Collections.Generic;
using System.Text;
using DAL;
namespace BLL
{
    public partial interface IItemBusiness : ICreate<ItemModel, bool>, IUpdate<ItemModel, bool>
    {
        ItemModel GetDatabyID(string id);
        List<ItemModel> GetDataAll();
        List<ItemModel> Search(int pageIndex, int pageSize, out long total, string item_group_id, string item_name);
    }
}
