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
    public class NguyenLieuController(INguyenLieuBusiness business) : ControllerBase
    {
        private readonly INguyenLieuBusiness _business = business;

        [Authorize(Roles = Role.Admin)]
        [HttpPost("create")]
        public IActionResult Create([FromBody] NguyenLieuModel model)
        {
            try
            {
                _business.Create(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("update")]
        public IActionResult Update([FromBody] NguyenLieuModel model)
        {
            try
            {
                _business.Update(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var id = GetString(formData, "nguyen_lieu_id");
                if (string.IsNullOrEmpty(id)) id = GetString(formData, "id");
                _business.Delete(id);
                return Ok(new { message = "Th�nh c�ng" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("get-by-id/{id}")]
        public NguyenLieuModel GetDatabyID(string id)
        {
            return _business.GetDatabyID(id);
        }

        [HttpGet("get-all")]
        public List<NguyenLieuModel> GetAll()
        {
            return _business.GetAll();
        }

        /// <summary>Nhập thêm hàng vào kho: cộng dồn số lượng, cập nhật giá nhập mới và ngày nhập cuối.</summary>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("nhap-them")]
        public IActionResult NhapThem([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var id = GetString(formData, "nguyen_lieu_id");
                double soLuong = GetDouble(formData, "so_luong");
                double giaNhap = GetDouble(formData, "gia_nhap");
                _business.NhapThem(id, soLuong, giaNhap);
                return Ok(new { message = "Th�nh c�ng" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
