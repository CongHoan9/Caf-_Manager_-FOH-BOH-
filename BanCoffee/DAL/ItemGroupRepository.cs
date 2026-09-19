using DAL.Helper;
using Model;
using System.Collections.Generic;
namespace DAL
{
    public partial class ItemGroupRepository(IDatabaseHelper dbHelper) : Repository(dbHelper), IItemGroupRepository
    {
        public List<ItemGroupModel> GetData() => ExecuteQuery<ItemGroupModel>("sp_item_group_get_data");
    }
}
