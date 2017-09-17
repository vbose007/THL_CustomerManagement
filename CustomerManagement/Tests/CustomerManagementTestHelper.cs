using System.Linq;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Helpers;
using CustomerManagement.API.Models;
using CustomerManagement.API.Services;
using CustomerManagement.Data;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;
using Moq;

namespace Tests
{
    public class CustomerManagementTestHelper
    {
        private CustomerManagementTests _customerManagementTests;

        public CustomerManagementTestHelper(CustomerManagementTests customerManagementTests)
        {
            _customerManagementTests = customerManagementTests;
        }

        public void AddCustomer(CustomerDto customer)
        {
            var service = SetupCustomersService();

            var customerDto = service.Create(customer);
        }

        public CustomersService SetupCustomersService(string currentUserName = "admin@admin.com", bool isAdmin = false)
        {
            var mockHttpContextHelper = SetUpMockHttpContextHelper(currentUserName, isAdmin);

            var mockRepo = SetupMockCustomerRepo();

            var service = new CustomersService(mockRepo.Object, mockHttpContextHelper.Object);
            return service;
        }

        public Mock<IHttpContextHelper> SetUpMockHttpContextHelper(string currentUserName = "admin@admin.com", bool isAdmin=false)
        {
            var mockHttpContextHelper = new Mock<IHttpContextHelper>();
            mockHttpContextHelper.Setup(x => x.CurrentUserName).Returns(currentUserName);
            mockHttpContextHelper.Setup(x => x.IsCurrentUserRole(It.IsAny<string>())).Returns((string r) =>
            {
                if (r.Equals(RolesEnum.Admin.ToString()) && isAdmin) return true;
                return false;
            });
            return mockHttpContextHelper;
        }

        public Mock<IRepository<Customer, CustomerModel>> SetupMockCustomerRepo()
        {
            var mockRepo = new Mock<IRepository<Customer, CustomerModel>>();
            mockRepo.Setup(x => x.GetAll()).Returns(_customerManagementTests.Repository);


            mockRepo.Setup(x => x.Get(It.IsAny<int>())).Returns((int Id) =>
            {
                return _customerManagementTests.Repository.FirstOrDefault(c => c.Id ==Id);
            });


            mockRepo.Setup(x => x.Add(It.IsAny<CustomerModel>())).Returns((CustomerModel c) =>
            {
                _customerManagementTests.Repository.Add(c);
                return c;
            });

            mockRepo.Setup(x => x.Update(It.IsAny<CustomerModel>())).Returns((CustomerModel c) =>
            {
                foreach (var customerModel in _customerManagementTests.Repository)
                {
                    if (customerModel.Id == c.Id)
                    {
                        customerModel.Name = c.Name;
                        customerModel.EmailAddress = c.EmailAddress;

                        return 1;
                    }
                }

                return 0;
            });


            mockRepo.Setup(x => x.Delete(It.IsAny<int>())).Returns((int id) =>
            {
                var item = _customerManagementTests.Repository.FirstOrDefault(x => x.Id == id);

                if (item == null) return 0;

                var index = _customerManagementTests.Repository.IndexOf(item);

                _customerManagementTests.Repository.RemoveAt(index);

                return 1;
            });


            return mockRepo;
        }

        public CustomerDto SetupCustomerDto(int id, string name, string email, string createdBy="admin@admin.com")
        {
            return  new CustomerDto()
            {
                Name = name,
                EmailAddress = email,
                Id = id,
                CreatedBy = createdBy
            };
        }
    }
}