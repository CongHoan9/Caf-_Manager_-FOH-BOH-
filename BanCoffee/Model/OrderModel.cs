using System;
using System.Collections.Generic;

#pragma warning disable IDE1006
namespace Model
{
    public class OrderModel
    {
        public string order_id { get; set; } = Guid.NewGuid().ToString();
        public string ban_id { get; set; }
        public string user_id { get; set; } // Phục vụ tạo đơn
        public DateTime time_created { get; set; } = DateTime.Now;
        public List<OrderItemModel> items { get; set; } = new List<OrderItemModel>();
        public string note { get; set; }
    }

    public class OrderItemModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString(); // Unique per line
        public string item_id { get; set; }
        public string item_name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string note { get; set; }
        
        // Trạng thái Bếp: 0 = Chờ làm, 1 = Đang làm, 2 = Đã xong, 3 = Đã giao
        public int state { get; set; } = 0; 
        public DateTime time_ordered { get; set; } = DateTime.Now;
    }
    
    public class KitchenTicketModel
    {
        public string order_id { get; set; }
        public string ban_id { get; set; }
        public OrderItemModel item { get; set; }
    }
}
