using DAL;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public partial class ItemGroupBusiness(IItemGroupRepository res) : Business<IItemGroupRepository>(res), IItemGroupBusiness
    {
        /// <summary>Dung cay phan cap nhom do uong tu danh sach phang tra ve tu SP.</summary>
        public List<ItemGroupModel> GetData()
        {
            var allItemGroups = _res.GetData();
            var lstParent = allItemGroups.Where(ds => ds.parent_item_group_id == null).OrderBy(s => s.seq_num).ToList();
            foreach (var item in lstParent)
            {
                item.children = GetHiearchyList(allItemGroups, item);
            }
            return lstParent;
        }

        public static List<ItemGroupModel> GetHiearchyList(List<ItemGroupModel> lstAll, ItemGroupModel node)
        {
            var lstChilds = lstAll.Where(ds => ds.parent_item_group_id == node.item_group_id).ToList();
            if (lstChilds.Count == 0)
                return null;
            for (int i = 0; i < lstChilds.Count; i++)
            {
                var childs = GetHiearchyList(lstAll, lstChilds[i]);
                lstChilds[i].type = (childs == null || childs.Count == 0) ? "leaf" : "";
                lstChilds[i].children = childs;
            }
            return lstChilds.OrderBy(s => s.seq_num).ToList();
        }
    }
}
