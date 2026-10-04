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
    public class CaLamController(ICaLamBusiness business) : ControllerBase
    {
        private readonly ICaLamBusiness _business = business;

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public CaLamModel Create([FromBody] CaLamModel model)
        {
            _business.Create(model);
            return model;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public CaLamModel Update([FromBody] CaLamModel model)
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
        public CaLamModel GetDatabyID(string id)
        {
            return _business.GetDatabyID(id);
        }

        [HttpGet("get-all")]
        public List<CaLamModel> GetAll()
        {
            return _business.GetAll();
        }
    }
}



