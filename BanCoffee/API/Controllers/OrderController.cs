using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderBusiness orderBusiness, IHoaDonBusiness hoaDonBusiness, IItemBusiness itemBusiness) : ControllerBusiness<IOrderBusiness>(orderBusiness)
    {
        private readonly IHoaDonBusiness _hoaDonBusiness = hoaDonBusiness;
        private readonly IItemBusiness _itemBusiness = itemBusiness;
        [Authorize(Roles = Role.Admin + "," + Role.ThuNgan + "," + Role.PhucVu)]
        [HttpPost("add-item")]
        public IActionResult AddItem([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                string ban_id = GetString(formData, "ban_id");
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var order = _Business.GetOrderByBanId(ban_id);
                if (order == null)
                {
                    order = new OrderModel
                    {
                        ban_id = ban_id,
                        user_id = currentUserId
                    };
                }

                var item = new OrderItemModel
                {
                    item_id = GetString(formData, "item_id"),
                    item_name = GetString(formData, "item_name"),
                    quantity = GetInt(formData, "quantity", 1),
                    price = (decimal)GetInt(formData, "price", 0), 
                    note = GetString(formData, "note", "")
                };
                
                bool ok = _itemBusiness.DeductInventoryForOrder(item.item_id, item.quantity, out string err);
                if (!ok) throw new Exception(err);
                
                _Business.AddItemToMemory(ban_id, currentUserId, item);

                return Ok(order);
            });
        }

        [Authorize(Roles = Role.Admin + "," + Role.ThuNgan + "," + Role.PhucVu)]
        [HttpGet("get-by-table/{banId}")]
        public IActionResult GetByTable(string banId)
        {
            return Execute(() =>
            {
                var order = _Business.GetOrderByBanId(banId);
                return Ok(order); 
            });
        }

        [Authorize(Roles = Role.Admin + "," + Role.ThuNgan)]
        [HttpPost("checkout")]
        public IActionResult Checkout([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                string ban_id = GetString(formData, "ban_id");
                string paymentMethod = GetString(formData, "payment_method", "Tien mat");
                string khuyen_mai_id = GetString(formData, "khuyen_mai_id", "");
                double giam_gia = (double)GetInt(formData, "giam_gia", 0);
                string ghi_chu = GetString(formData, "ghi_chu", "");

                var order = _Business.GetOrderByBanId(ban_id);
                if (order == null || !order.items.Any())
                    return BadRequest(new { message = "Ban chua co order hoac khong co mon" });

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                double tongTien = (double)order.items.Sum(x => x.quantity * x.price);
                double thanhTien = tongTien - giam_gia;
                if (thanhTien < 0) thanhTien = 0;

                var invoice = new HoaDonModel
                {
                    ma_hoa_don = Guid.NewGuid().ToString(),
                    ngay_tao = DateTime.Now,
                    tong_tien = tongTien,
                    giam_gia = giam_gia,
                    thanh_tien = thanhTien,
                    hinh_thuc_thanh_toan = paymentMethod,
                    ghi_chu = ghi_chu,
                    user_id = currentUserId ?? order.user_id,
                    ban_id = ban_id,
                    khuyen_mai_id = string.IsNullOrEmpty(khuyen_mai_id) ? null : khuyen_mai_id,
                    listjson_chitiet = order.items.Select(i => new ChiTietHoaDonModel
                    {
                        ma_chi_tiet = Guid.NewGuid().ToString(),
                        item_id = i.item_id,
                        item_name = i.item_name,
                        so_luong = i.quantity,
                        don_gia = (double)i.price,
                        status = 1 
                    }).ToList()
                };

                _hoaDonBusiness.Create(invoice);
                _Business.ClearOrder(ban_id);

                return Ok(new { message = "Thanh toan thanh cong", hoadon = invoice });
            });
        }
        
        [Authorize(Roles = Role.Admin + "," + Role.PhucVu)]
        [HttpPost("clear/{banId}")]
        public IActionResult ClearOrder(string banId)
        {
            return Execute(() =>
            {
                _Business.ClearOrder(banId);
                return Ok(new { message = "Da huy order" });
            });
        }
    }
}
