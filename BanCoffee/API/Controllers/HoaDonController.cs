using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class HoaDonController(IHoaDonBusiness hoaDonBusiness, ILogger<ItemGroupBusiness> logger) : ControllerBusiness<IHoaDonBusiness>(hoaDonBusiness), ICreate<HoaDonModel, HoaDonModel>
    {
        private readonly ILogger<ItemGroupBusiness> _logger = logger;
        [Route("create-hoa-don")]
        [HttpPost]
        public HoaDonModel Create([FromBody] HoaDonModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(currentUserId)) {
                model.user_id = currentUserId;
            }
            _Business.Create(model);
            return model;
        }
        [Route("update-hoa-don")]
        [HttpPost]
        public IActionResult UpdateItem([FromBody] HoaDonModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole(Model.Role.Admin);
            var existing = _Business.GetDatabyID(model.ma_hoa_don);
            if (existing == null)
            {
                return NotFound();
            }
            if (!isAdmin && existing.user_id != currentUserId)
            {
                return StatusCode(403, new { message = "B?n không có quy?n s?a hóa don c?a ngu?i khác" });
            }
            model.user_id = existing.user_id;
            _Business.Update(model);
            return Ok(model);
        }
        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                int page = GetInt(formData, "page", 1);
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole(Model.Role.Admin);
                int pageSize = GetInt(formData, "pageSize", 10);
                string hoten = GetString(formData, "hoten", "");
                string diachi = GetString(formData, "diachi", "");
                var data = _Business.Search(page, pageSize, out long total, hoten, diachi, isAdmin ? "" : currentUserId);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }
        [Route("get-by-id/{id}")]
        [HttpGet]
        public HoaDonModel GetDatabyID(string id)
        {
            var kq = _Business.GetDatabyID(id);
            _logger.LogInformation(MessageConvert.SerializeObject(kq));
            return _Business.GetDatabyID(id);
        }
        [Authorize(Roles = Role.Admin)]
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteHoaDon([FromBody] Dictionary<string, object> formData)
        {
            string ma_hoa_don = GetString(formData, "ma_hoa_don");
            _Business.Delete(ma_hoa_don);
            return Ok(new { message = "Thành công" });
        }
    }
}












