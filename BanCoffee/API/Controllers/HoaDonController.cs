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
    [ApiController]
    public class HoaDonController : ControllerBusiness<IHoaDonBusiness>, ICreate<HoaDonModel, HoaDonModel>
    {
        private readonly ILogger<ItemGroupBusiness> _logger;
        public HoaDonController(IHoaDonBusiness hoaDonBusiness, ILogger<ItemGroupBusiness> logger) : base(hoaDonBusiness)
        {
            _logger = logger;
        }

        [Route("create-hoa-don")]
        [HttpPost]
        public HoaDonModel Create([FromBody] HoaDonModel model)
        {
            _Business.Create(model);
            return model;
        }

        [Route("update-hoa-don")]
        [HttpPost]
        public HoaDonModel UpdateItem([FromBody] HoaDonModel model)
        {
            _Business.Update(model);
            return model;
        }

        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                if (int.TryParse(formData["page"].ToString(), out int page) && 
                    int.TryParse(formData["pageSize"].ToString(), out int pageSize))
                {
                    string hoten = "";
                    if (formData.ContainsKey("hoten") && 
                        !string.IsNullOrEmpty(Convert.ToString(formData["hoten"]))) 
                    { 
                        hoten = Convert.ToString(formData["hoten"]) ?? ""; 
                    }
                    string diachi = "";
                    if (formData.ContainsKey("diachi") && 
                        !string.IsNullOrEmpty(Convert.ToString(formData["diachi"]))) 
                    { 
                        diachi = Convert.ToString(formData["diachi"]) ?? ""; 
                    }
                    var data = _Business.Search(page, pageSize, out long total, hoten, diachi);
                    response.TotalItems = total;
                    response.Data = data;
                    response.Page = page;
                    response.PageSize = pageSize;
                }
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

        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] Dictionary<string, object> formData)
        {
            string ma_hoa_don = "";
            if (formData.ContainsKey("ma_hoa_don") && !string.IsNullOrEmpty(Convert.ToString(formData["ma_hoa_don"]))) { ma_hoa_don = Convert.ToString(formData["ma_hoa_don"]) ?? ""; }
            _Business.Delete(ma_hoa_don);
            return Ok();
        }
    }
}
