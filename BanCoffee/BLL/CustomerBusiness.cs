using DAL;
using Model;
using System;

namespace BLL
{
    public partial class CustomerBusiness(ICustomerRepository res) : Business<ICustomerRepository>(res), ICustomerBusiness
    {
        public bool Create(CustomerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.customer_email) || !model.customer_email.Contains("@"))
                throw new Exception("Email khach hang khong hop le.");
            return _res.Create(model);
        }
    }
}
