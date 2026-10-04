using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Model;

namespace API.Controllers
{
    [Authorize]
    //[ApiKey]
    //http://localhost:52872/api/Item/get-by-id/1
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController(IItemBusiness itemBusiness, IMemoryCache memoryCache) : ControllerBusiness<IItemBusiness>(itemBusiness)
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        [AllowAnonymous]
        [Route("get-by-id/{id}")]
        [HttpGet]
        public ItemModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }
        [Authorize(Roles = Role.Admin)]
        [Route("create-item")]
        [HttpPost]
        public ItemModel CreateItem([FromBody] ItemModel model)
        {
            _memoryCache.Remove("all-item");
            model.item_id = Guid.NewGuid().ToString();
            _Business.Create(model);
            return model;
        }
        [Authorize(Roles = Role.Admin)]
        [Route("update-item")]
        [HttpPost]
        public ItemModel UpdateItem([FromBody] ItemModel model)
        {
            _memoryCache.Remove("all-item");
            _Business.Update(model);
            return model;
        }
        [Authorize(Roles = Role.Admin)]
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteItem([FromBody] Dictionary<string, object> formData)
        {
            string item_id = "";
            if (formData.TryGetValue("item_id", out object value) &&
                !string.IsNullOrEmpty(Convert.ToString(value)))
            {
                item_id = Convert.ToString(value);
            }
            _Business.Delete(item_id);
            return Ok(new { message = "Thành công" });
        }
        [AllowAnonymous]
        [Route("get-all")]
        [HttpGet]
        public IEnumerable<ItemModel> GetDatabAll()
        {
            var list = _memoryCache.Get<List<ItemModel>>("all-item");
            if (list == null)
            {
                var result = _Business.GetDataAll();
                _memoryCache.Set("all-item", result, TimeSpan.FromMinutes(60));
                return result;
            }
            else
            {
                return list;
            }
        }
        [AllowAnonymous]
        [Route("search")]
        [HttpPost]
        public ResponseModel Search([FromBody] Dictionary<string, object> formData)
        {
            var response = new ResponseModel();
            try
            {
                int page = GetInt(formData, "page", 1);
                int pageSize = GetInt(formData, "pageSize", 10);
                string item_group_id = GetString(formData, "item_group_id", "");
                string item_name = GetString(formData, "item_name", "");

                var data = _Business.Search(page, pageSize, out long total, item_group_id, item_name);
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







