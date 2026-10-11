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
    public class BanController(IBanBusiness business) : ControllerBusiness<IBanBusiness>(business)
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create-ban")]
        public BanModel CreateBan([FromBody] BanModel model)
        {
            _Business.Create(model);
            return model;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update-ban")]
        public BanModel UpdateBan([FromBody] BanModel model)
        {
            _Business.Update(model);
            return model;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete-ban")]
        public IActionResult DeleteBan([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "id");
                _Business.Delete(id);
                return Ok(new { message = "Thanh cong" });
            });
        }

        [HttpGet("get-by-id/{id}")]
        public BanModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }

        [HttpGet("get-all")]
        public List<BanModel> GetAll()
        {
            return _Business.GetAll();
        }

        [HttpPost("update-trang-thai")]
        public IActionResult UpdateTrangThai([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "ban_id");
                var trang_thai = GetString(formData, "trang_thai");
                _Business.UpdateTrangThai(id, trang_thai);
                return Ok(new { message = "Thanh cong" });
            });
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
                string filter = GetString(formData, "filterName", "");
                var data = _Business.Search(page, pageSize, out long total, filter);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
                return response;
            });
        }
    }
}



