using CustomerManagement.API.Helpers;
using CustomerManagement.API.Interfaces;
using CustomerManagement.API.Services;
using CustomerManagement.Data;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;
using Microsoft.Practices.Unity;

namespace CustomerManagement.API.Config
{
    public class DependencyConfig
    {
        public static UnityContainer  RegisterDependencies()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerService, CustomersService>();
            container.RegisterType<IRepository<Customer, CustomerModel>, CustomerRepository>();
            container.RegisterType<IHttpContextHelper, HttpContextHelper>();

            return container;
        }
    }
}