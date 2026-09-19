using DAL.Helper;
using Model;

namespace DAL
{
    public partial class CustomerRepository : Repository, ICustomerRepository
    {
        public CustomerRepository(IDatabaseHelper dbHelper) : base(dbHelper) { }
        public bool Create(CustomerModel model) => ExecuteTransaction("sp_customer_create",
                                                                      "@customer_email", model.customer_email,
                                                                      "@customer_password", model.customer_password);
    }
}
