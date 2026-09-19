using DAL.Helper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public partial class ItemRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), IItemRepository
    {
        public bool Create(ItemModel model) => ExecuteTransaction("sp_item_create",
                                                                  "@item_id", model.item_id,
                                                                  "@item_group_id", model.item_group_id,
                                                                  "@item_image", model.item_image,
                                                                  "@item_name", model.item_name,
                                                                  "@item_price", model.item_price);
        public bool Update(ItemModel model) => ExecuteTransaction("sp_item_update",
                                                                  "@item_id", model.item_id,
                                                                  "@item_group_id", model.item_group_id,
                                                                  "@item_image", model.item_image,
                                                                  "@item_name", model.item_name,
                                                                  "@item_price", model.item_price);
        public bool Delete(string id) => ExecuteTransaction("sp_item_delete", "@item_id", id);
        public ItemModel GetDatabyID(string id) => ExecuteQuery<ItemModel>("sp_item_get_by_id", "@item_id", id).FirstOrDefault();
        public List<ItemModel> GetDataAll() => ExecuteQuery<ItemModel>("sp_item_all");
        public List<ItemModel> Search(int pageIndex, int pageSize, out long total, string item_group_id, string item_name)
        {
            total = 0;
            return ExecuteQuery<ItemModel>("sp_item_search",
                                           "@page_index", pageIndex,
                                           "@page_size", pageSize,
                                           "@item_name", item_name,
                                           "@item_group_id", item_group_id);
        }
    }
}
