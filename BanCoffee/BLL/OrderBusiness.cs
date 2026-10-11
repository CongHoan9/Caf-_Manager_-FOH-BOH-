using Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class OrderBusiness : IOrderBusiness
    {
        private readonly ConcurrentDictionary<string, OrderModel> _ordersByBanId = new();

        public OrderModel CreateOrUpdateOrder(OrderModel order)
        {
            _ordersByBanId[order.ban_id] = order;
            return order;
        }

        public OrderModel GetOrderByBanId(string ban_id)
        {
            if (_ordersByBanId.TryGetValue(ban_id, out var order))
                return order;
            return null;
        }

        public List<KitchenTicketModel> GetKitchenQueue()
        {
            var queue = new List<KitchenTicketModel>();
            foreach (var order in _ordersByBanId.Values)
            {
                foreach (var item in order.items)
                {
                    if (item.state < 3) 
                    {
                        queue.Add(new KitchenTicketModel
                        {
                            order_id = order.order_id,
                            ban_id = order.ban_id,
                            item = item
                        });
                    }
                }
            }
            return [.. queue.OrderBy(q => q.item.time_ordered)];
        }

        public bool UpdateItemState(string order_id, string item_id, int newState)
        {
            var order = _ordersByBanId.Values.FirstOrDefault(o => o.order_id == order_id);
            if (order != null)
            {
                var item = order.items.FirstOrDefault(i => i.id == item_id);
                if (item != null)
                {
                    item.state = newState;
                    return true;
                }
            }
            return false;
        }

        public bool ClearOrder(string ban_id)
        {
            return _ordersByBanId.TryRemove(ban_id, out _);
        }

        public void AddItemToMemory(string ban_id, string user_id, OrderItemModel item)
        {
            var order = GetOrderByBanId(ban_id);
            if (order == null)
            {
                order = new OrderModel
                {
                    ban_id = ban_id,
                    user_id = user_id
                };
                _ordersByBanId[ban_id] = order;
            }
            order.items.Add(item);
        }
    }
}
