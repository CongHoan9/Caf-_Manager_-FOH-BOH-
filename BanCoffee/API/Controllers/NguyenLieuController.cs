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
    public class NguyenLieuController(INguyenLieuBusiness business) : ControllerBusiness<INguyenLieuBusiness>(business)
    {
        [Authorize(Roles = Role.Admin)]
        [HttpPost("create")]
        public IActionResult Create([FromBody] NguyenLieuModel model)
        {
            return Execute(() =>
            {
                _Business.Create(model);
                return Ok(model);
            });
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("update")]
        public IActionResult Update([FromBody] NguyenLieuModel model)
        {
            return Execute(() =>
            {
                _Business.Update(model);
                return Ok(model);
            });
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "nguyen_lieu_id");
                if (string.IsNullOrEmpty(id)) id = GetString(formData, "id");
                _Business.Delete(id);
                return Ok(new { message = "Thanh cong" });
            });
        }

        [HttpGet("get-by-id/{id}")]
        public NguyenLieuModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }

        [HttpGet("get-all")]
        public List<NguyenLieuModel> GetAll()
        {
            return _Business.GetAll();
        }

        /// <summary>Nhập thêm hàng vào kho: cộng dồn số lượng, cập nhật giá nhập mới và ngày nhập cuối.</summary>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("nhap-them")]
        public IActionResult NhapThem([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "nguyen_lieu_id");
                double soLuong = GetDouble(formData, "so_luong");
                double giaNhap = GetDouble(formData, "gia_nhap");
                _Business.NhapThem(id, soLuong, giaNhap);
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
