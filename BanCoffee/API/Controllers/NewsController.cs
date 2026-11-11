using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController(INewsBusiness newsBusiness) : ControllerBusiness<INewsBusiness>(newsBusiness)
    {
        [Authorize(Roles = Role.Admin)]
        [Route("create-news")]
        [HttpPost]
        public NewsModel CreateNews([FromBody] NewsModel model)
        {
            model.news_id = Guid.NewGuid().ToString();
            _Business.Create(model);
            return model;
        }

        [AllowAnonymous]
        [Route("get-all")]
        [HttpGet]
        public List<NewsModel> GetAll()
        {
            return _Business.GetDataAll();
        }

        [AllowAnonymous]
        [Route("get-by-id/{id}")]
        [HttpGet]
        public NewsModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }

        [Authorize(Roles = Role.Admin)]
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteNews([FromBody] Dictionary<string, object> formData)
        {
            return Execute(() =>
            {
                string news_id = GetString(formData, "news_id");
                if (string.IsNullOrEmpty(news_id)) news_id = GetString(formData, "id");
                _Business.Delete(news_id);
                return Ok(new { message = "Thanh cong" });
            });
        }
    }
}
