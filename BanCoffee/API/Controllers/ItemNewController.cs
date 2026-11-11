using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemNewController(IItemBusiness itemBusiness) : ControllerBusiness<IItemBusiness>(itemBusiness)
    {
        [Authorize]
        [Route("get-by-id/{id}")]
        [HttpGet]
        public ItemModel GetDatabyID(string id)
        {
            return _Business.GetDatabyID(id);
        }
    }
}
