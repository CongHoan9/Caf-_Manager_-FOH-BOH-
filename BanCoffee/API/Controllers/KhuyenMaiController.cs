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
    public class KhuyenMaiController(IKhuyenMaiBusiness business) : ControllerBusiness<IKhuyenMaiBusiness>(business)
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public KhuyenMaiModel Create([FromBody] KhuyenMaiModel model)
        {
            _Business.Create(model);
            return model;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public KhuyenMaiModel Update([FromBody] KhuyenMaiModel model)
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
        public KhuyenMaiModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }
        [HttpGet("get-all")]
        public List<KhuyenMaiModel> GetAll()
        {
            return _Business.GetAll();
        }
        [HttpGet("get-by-code/{code}")]
        public KhuyenMaiModel GetByCode(string code)
        {
            return _Business.GetByCode(code);
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
                string ten = GetString(formData, "ten", "");
                string trang_thai = GetString(formData, "trang_thai", "");
                
                var data = _Business.Search(page, pageSize, out long total, ten, trang_thai);
                response.TotalItems = total;
                response.Data = data;
                response.Page = page;
                response.PageSize = pageSize;
                return response;
            });
        }
    }
}



