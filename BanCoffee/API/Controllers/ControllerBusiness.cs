using Microsoft.AspNetCore.Mvc;
namespace API
{
    public abstract class ControllerBusiness<Business> : ControllerBase
    {
        protected readonly Business _Business;
        public ControllerBusiness(Business business)
        {
            _Business = business;
        }
    }
}
