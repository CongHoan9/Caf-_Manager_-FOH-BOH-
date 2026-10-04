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
    public class KhuyenMaiController(IKhuyenMaiBusiness business) : ControllerBase
    {
        private readonly IKhuyenMaiBusiness _business = business;
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public KhuyenMaiModel Create([FromBody] KhuyenMaiModel model)
        {
            _business.Create(model);
            return model;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public KhuyenMaiModel Update([FromBody] KhuyenMaiModel model)
        {
            _business.Update(model);
            return model;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData)
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
        public KhuyenMaiModel GetDatabyID(string id)
        {
            return _business.GetDatabyID(id);
        }
        [HttpGet("get-all")]
        public List<KhuyenMaiModel> GetAll()
        {
            return _business.GetAll();
        }
        [HttpGet("get-by-code/{code}")]
        public KhuyenMaiModel GetByCode(string code)
        {
            return _business.GetByCode(code);
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
                string ten = GetString(formData, "ten", "");
                string trang_thai = GetString(formData, "trang_thai", "");

                var data = _business.Search(page, pageSize, out long total, ten, trang_thai);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return response;
        }
    }
}



