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
    //[Authorize]
    //[ApiKey]
    //http://localhost:52872/api/Item/get-by-id/1
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBusiness<IItemBusiness>
    {
        private readonly IMemoryCache _memoryCache;
        public ItemController(IItemBusiness itemBusiness, IMemoryCache memoryCache) : base(itemBusiness)
        {
            _memoryCache = memoryCache;
        }

        [Route("get-by-id/{id}")]
        [HttpGet]
        public ItemModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }

        [Route("create-item")]
        [HttpPost]
        public ItemModel CreateItem([FromBody] ItemModel model)
        {
            //_memoryCache.Remove("all-item");
            model.item_id = Guid.NewGuid().ToString();
            _Business.Create(model);
            return model;
        }
        [Route("update-item")]
        [HttpPost]
        public ItemModel UpdateItem([FromBody] ItemModel model)
        {
            //_memoryCache.Remove("all-item");
            _Business.Update(model);
            return model;
        }
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
                    string item_group_id = "";
                    if (formData.TryGetValue("item_group_id", out object value) && 
                        !string.IsNullOrEmpty(Convert.ToString(value))) 
                    { 
                        item_group_id = Convert.ToString(value); 
                    }
                    string item_name = "";
                    if (formData.TryGetValue("item_name", out object value1) && 
                       !string.IsNullOrEmpty(Convert.ToString(value1))) 
                    { 
                        item_name = Convert.ToString(value1); 
                    }
                    var data = _Business.Search(page, pageSize, out long total, item_group_id, item_name);
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

    }
}
