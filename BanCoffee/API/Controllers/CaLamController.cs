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
    public class CaLamController(ICaLamBusiness business) : ControllerBusiness<ICaLamBusiness>(business)
    {
        [Authorize(Roles = Role.Admin)]
        [HttpPost("create")]
        public CaLamModel Create([FromBody] CaLamModel model)
        {
            _Business.Create(model);
            return model;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("update")]
        public CaLamModel Update([FromBody] CaLamModel model)
        {
            _Business.Update(model);
            return model;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("delete")]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                var id = GetString(formData, "ca_lam_id");
                if (string.IsNullOrEmpty(id)) id = GetString(formData, "id");
                _Business.Delete(id);
                return Ok(new { message = "Thanh cong" });
            });
        }

        [Authorize]
        [HttpGet("get-by-id/{id}")]
        public CaLamModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }

        [Authorize]
        [HttpGet("get-all")]
        public List<CaLamModel> GetAll()
        {
            return _Business.GetAll();
        }
    }
}
