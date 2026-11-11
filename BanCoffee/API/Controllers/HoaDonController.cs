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

        [Authorize]
        [Route("create-hoa-don")]
        [HttpPost]
        public HoaDonModel Create([FromBody] HoaDonModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                model.user_id = currentUserId;
            }
            if (model.ngay_tao == default || model.ngay_tao < new DateTime(1753, 1, 1))
            {
                model.ngay_tao = DateTime.Now;
            }
            _Business.Create(model);
            return model;
        }

        [Authorize]
        [Route("update-hoa-don")]
        [HttpPost]
        public IActionResult UpdateItem([FromBody] HoaDonModel model)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole(Model.Role.Admin);
            var isThuNgan = User.IsInRole(Model.Role.ThuNgan);
            var existing = _Business.GetDatabyID(model.ma_hoa_don);
            if (existing == null)
            {
                return NotFound(new { message = "Hoa don khong ton tai" });
            }
            if (!isAdmin && !isThuNgan && existing.user_id != currentUserId)
            {
                return StatusCode(403, new { message = "Ban khong co quyen nay" });
            }
            model.user_id = existing.user_id;
            if (model.ngay_tao == default || model.ngay_tao < new DateTime(1753, 1, 1))
            {
                model.ngay_tao = existing.ngay_tao > new DateTime(1753, 1, 1) ? existing.ngay_tao : DateTime.Now;
            }
            _Business.Update(model);
            return Ok(model);
        }

        [Authorize]
        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var response = new ResponseModel();
                int page = GetInt(formData, "page", 1);
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole(Role.Admin);
                var isThuNgan = User.IsInRole(Role.ThuNgan);
                int pageSize = GetInt(formData, "pageSize", 10);
                string hoten = GetString(formData, "hoten", "");
                string diachi = GetString(formData, "diachi", "");
                var data = _Business.Search(page, pageSize, out long total, hoten, diachi, (isAdmin || isThuNgan) ? "" : currentUserId);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
                return response;
            });
        }

        [Authorize]
        [Route("get-by-id/{id}")]
        [HttpGet]
        public HoaDonModel GetDatabyID(string id)
        {
            var kq = _Business.GetDatabyID(id);
            _logger.LogInformation(MessageConvert.SerializeObject(kq));
            return kq;
        }

        [Authorize(Roles = Role.Admin)]
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteHoaDon([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                string ma_hoa_don = GetString(formData, "ma_hoa_don");
                if (string.IsNullOrEmpty(ma_hoa_don)) ma_hoa_don = GetString(formData, "id");
                _Business.Delete(ma_hoa_don);
                return Ok(new { message = "Thanh cong" });
            });
        }
    }
}
