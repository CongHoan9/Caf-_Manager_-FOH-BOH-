using Model;
namespace BLL
{
    public partial class CustomerBusiness(ICustomerBusiness res) : Business<ICustomerBusiness>(res), ICustomerBusiness
    {
        public bool Create(CustomerModel model)
        {
            return _res.Create(model);
        }
    }

}
