using Microsoft.AspNetCore.Mvc;
namespace API
{
    public abstract class ControllerBusiness<Business>(Business business) : ControllerBase
    {
        protected readonly Business _Business = business;
    }
}

