using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerBusiness customerBusiness) : ControllerBusiness<ICustomerBusiness>(customerBusiness), ICreate<CustomerModel, CustomerModel>
    {
        [Authorize]
        [Route("create-item")]
        [HttpPost]
        public CustomerModel Create([FromBody] CustomerModel model)
        {
            _Business.Create(model);
            return model;
        }
    }
}
