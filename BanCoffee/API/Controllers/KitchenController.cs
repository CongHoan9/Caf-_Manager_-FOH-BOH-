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
        [Authorize(Roles = Role.Admin + "," + Role.Bep + "," + Role.PhucVu + "," + Role.Staff)]
        [HttpGet("queue")]
        public IActionResult GetQueue()
        {
            return Execute(() =>
            {
                var queue = _Business.GetKitchenQueue();
                return Ok(queue);
            });
        }

        [Authorize(Roles = Role.Admin + "," + Role.Bep + "," + Role.PhucVu + "," + Role.Staff)]
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
                    return Ok(new { message = "Cập nhật thành công" });
                
                return BadRequest(new { message = "Không tìm thấy món hoặc đơn hàng" });
            });
        }
    }
}
