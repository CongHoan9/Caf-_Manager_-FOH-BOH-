using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LichLamViecController(ILichLamViecBusiness business) : ControllerBusiness<ILichLamViecBusiness>(business)
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public LichLamViecModel Create([FromBody] LichLamViecModel model)
        {
            _Business.Create(model);
            return model;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public LichLamViecModel Update([FromBody] LichLamViecModel model)
        {
            _Business.Update(model);
            return model;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "id");
                _Business.Delete(id);
                return Ok(new { message = "Thanh cong" });
            });
        }
        [HttpGet("get-by-id/{id}")]
        public LichLamViecModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }
        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var response = new ResponseModel();
                int page = GetInt(formData, "page", 1);
                int pageSize = GetInt(formData, "pageSize", 10);
                string user_id = GetString(formData, "user_id", "");
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!User.IsInRole(Model.Role.Admin)) user_id = currentUserId;
                DateTime? ngay_lam = GetDateTime(formData, "ngay_lam");
                string ca_lam_id = GetString(formData, "ca_lam_id", "");
                
                var data = _Business.Search(page, pageSize, out long total, user_id, ngay_lam, ca_lam_id);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
                return response;
            });
        }
    }
}








