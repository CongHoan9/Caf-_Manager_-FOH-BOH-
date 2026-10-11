using Model;
using System.Collections.Generic;

namespace BLL
{
    public interface IOrderBusiness
    {
        OrderModel CreateOrUpdateOrder(OrderModel order);
        OrderModel GetOrderByBanId(string ban_id);
        List<KitchenTicketModel> GetKitchenQueue();
        bool UpdateItemState(string order_id, string item_id, int newState);
        bool ClearOrder(string ban_id);
        
        void AddItemToMemory(string ban_id, string user_id, OrderItemModel item);
    }
}
