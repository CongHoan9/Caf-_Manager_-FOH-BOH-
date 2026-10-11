using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Collections.Generic;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class KitchenController(IOrderBusiness orderBusiness) : ControllerBusiness<IOrderBusiness>(orderBusiness)
    {
        // Bếp lấy danh sách món cần làm
        [Authorize(Roles = Role.Admin + "," + Role.Bep)]
        [HttpGet("queue")]
        public IActionResult GetQueue()
        {
            return Execute(() =>
            {
                var queue = _Business.GetKitchenQueue();
                return Ok(queue);
            });
        }

        // Bếp/Phục vụ chuyển trạng thái món (0: chờ, 1: đang làm, 2: xong, 3: đã giao)
        [Authorize(Roles = Role.Admin + "," + Role.Bep + "," + Role.PhucVu)]
        [HttpPost("update-state")]
        public IActionResult UpdateState([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                string order_id = GetString(formData, "order_id");
                string item_id = GetString(formData, "item_id");
                int state = GetInt(formData, "state", 0);

                bool success = _Business.UpdateItemState(order_id, item_id, state);
                if (success)
                    return Ok(new { message = "Cap nhat thanh cong" });
                
                return BadRequest(new { message = "Khong tim thay mon hoac don hang" });
            });
        }
    }
}
