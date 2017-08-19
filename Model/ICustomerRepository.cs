using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageVerification.Model
{
    public interface ICustomerRepository
    {

        ObservableCollection<Customer> GetCustomers();
        void UpdateCustomer(Customer selectedCustomer);
        void DeleteCustomer(Customer selectedCustomer);


    }
}
