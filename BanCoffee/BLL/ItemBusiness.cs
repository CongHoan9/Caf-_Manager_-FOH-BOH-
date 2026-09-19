using DAL;
using Model;
using System;
using System.Collections.Generic;

namespace BLL
{
    public partial class ItemBusiness(IItemRepository res) : Business<IItemRepository>(res), IItemBusiness
    {
        public bool Create(ItemModel model)
        {
            model.item_id = model.item_id ?? Guid.NewGuid().ToString();
            return _res.Create(model);
        }

        public bool Update(ItemModel model)
        {
            return _res.Update(model);
        }

        public bool Delete(string id)
        {
            return _res.Delete(id);
        }

        public ItemModel GetDatabyID(string id)
        {
            return _res.GetDatabyID(id);
        }

        public List<ItemModel> GetDataAll()
        {
            return _res.GetDataAll();
        }

        public List<ItemModel> Search(int pageIndex, int pageSize, out long total, string item_group_id, string item_name)
        {
            return _res.Search(pageIndex, pageSize, out total, item_group_id, item_name);
        }
    }
}
