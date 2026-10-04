using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerBusiness customerBusiness) : ControllerBusiness<ICustomerBusiness>(customerBusiness), ICreate<CustomerModel, CustomerModel>
    {
        [Route("create-item")]
        [HttpPost]
        public CustomerModel Create([FromBody] CustomerModel model)
        {
            _Business.Create(model);
            return model;
        } 
    }
}

