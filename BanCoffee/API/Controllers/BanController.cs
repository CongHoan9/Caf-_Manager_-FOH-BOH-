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
    public class BanController(IBanBusiness business) : ControllerBase
    {
        private readonly IBanBusiness _business = business;

        [Authorize(Roles = "Admin")]
        [HttpPost("create-ban")]
        public BanModel CreateBan([FromBody] BanModel model)
        {
            _business.Create(model);
            return model;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update-ban")]
        public BanModel UpdateBan([FromBody] BanModel model)
        {
            _business.Update(model);
            return model;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete-ban")]
        public IActionResult DeleteBan([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var id = GetString(formData, "id");
                _business.Delete(id);
                return Ok(new { message = "Thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-id/{id}")]
        public BanModel GetDatabyID(string id)
        {
            return _business.GetDatabyID(id);
        }

        [HttpGet("get-all")]
        public List<BanModel> GetAll()
        {
            return _business.GetAll();
        }

        [HttpPost("update-trang-thai")]
        public IActionResult UpdateTrangThai([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var id = GetString(formData, "ban_id");
                var trang_thai = GetString(formData, "trang_thai");
                _business.UpdateTrangThai(id, trang_thai);
                return Ok(new { message = "Thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("search")]
        [HttpPost]
                public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                int page = GetInt(formData, "page", 1);
                int pageSize = GetInt(formData, "pageSize", 10);
                string filter = GetString(formData, "filterName", "");

                var data = _business.Search(page, pageSize, out long total, filter);
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
    }
}



