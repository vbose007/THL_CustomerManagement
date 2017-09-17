using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerManagement.API.Config;
using CustomerManagement.API.CustomExceptions;
using CustomerManagement.API.DTOs;
using CustomerManagement.Data;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;
using NUnit.Framework;
using CustomerManagement.API.Services;
using Moq;
using CustomerManagement.API.Helpers;
using CustomerManagement.Data.Config;

namespace Tests
{
    [TestFixture]
    public class CustomerManagementTests
    {
        private readonly CustomerManagementTestHelper _customerManagementTestHelper;

        public IList<CustomerModel> Repository { get; set;}

        public CustomerManagementTests()
        {
            _customerManagementTestHelper = new CustomerManagementTestHelper(this);
        }

        [SetUp]
        public void Setup()
        {
            Repository = new List<CustomerModel>();

            ApiMapperConfig.ConfigureMappings();
        }

#region Basic CRUD tests

        [Test]
        public void CanCreateCustomer()
        {
            //Arrange
            Repository.Clear();
            Assert.IsTrue(Repository.Count == 0);
            var customer = _customerManagementTestHelper.SetupCustomerDto(0, "Test1", "Test1@Test1.com");

            //Act
            _customerManagementTestHelper.AddCustomer(customer);

            //Assert
            Assert.IsTrue(Repository.Any(x => x.Name.Equals(customer.Name)));
        }

        [Test]
        public void CanUpdateCustomer()
        {
            //Arrange
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com");
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            var service = _customerManagementTestHelper.SetupCustomersService();

            //Act
            
            //Update
            customerDto.Name = "Test12";
            bool updated = service.Update(customerDto);

            //Assert
            Assert.IsTrue(updated);
            Assert.IsTrue(Repository.Any(x => x.Id == customerDto.Id && x.Name == "Test12"));
        }



        [Test]
        public void CanDeleteCustomer()
        {
            //Arrange
            Repository.Clear();
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com");
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            var service = _customerManagementTestHelper.SetupCustomersService();

            //Act

            //Delete
            bool deleted = service.Delete(customerDto);

            //Assert
            Assert.IsTrue(deleted);
            Assert.IsFalse(Repository.Any(x => x.Id == customerDto.Id));
        }

        #endregion




#region Validation Tests

        [Test]
        public void CannotCreateCustomerWithoutName()
        {
            //Arrange
            Repository.Clear();
            Assert.IsTrue(Repository.Count == 0);
            var customer = _customerManagementTestHelper.SetupCustomerDto(0, "", "Test1@Test1.com");

            //Act
             TestDelegate addCustomer = () =>_customerManagementTestHelper.AddCustomer(customer);

            //Assert
            Assert.Throws<BadRequestException>(addCustomer);
        }

        [Test]
        public void CannotUpdateCustomerWithNoName()
        {
            //Arrange
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com");
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            var service = _customerManagementTestHelper.SetupCustomersService();

            //Act

            //Update
            customerDto.Name = "";
            TestDelegate updateCustomer  =  () => service.Update(customerDto);

            //Assert
            Assert.Throws<BadRequestException>(updateCustomer);
        }

        #endregion



 #region AuthorizationTests

        [Test]
        public void CannotUpdateCustomerCreatedByAnotherUser()
        {
            //Arrange
            var user1 = "user1@user1.com";
            var user2 = "user2@user2.com";
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com", user1);
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            //Setup service for user2
            var service = _customerManagementTestHelper.SetupCustomersService(user2);

            //Act
            //Update customer created by user1
            customerDto.Name = "Test12";
            TestDelegate updateCustomer = () => service.Update(customerDto);

            //Assert
            Assert.Throws<AuthorizationException>(updateCustomer);
        }


        [Test]
        public void CannotDeleteCustomerCreatedByAnotherUser()
        {
            //Arrange
            var user1 = "user1@user1.com";
            var user2 = "user2@user2.com";
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com", user1);
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            //Setup service for user2
            var service = _customerManagementTestHelper.SetupCustomersService(user2);

            //Act

            //Delete customer created by user1
            TestDelegate deleteCustomer = () => service.Delete(customerDto); ;

            //Assert
            Assert.Throws<AuthorizationException>(deleteCustomer);
        }


        [Test]
        public void AdminUserCanUpdateCustomerCreatedByAnotherUser()
        {
            //Arrange
            var user1 = "user1@user1.com";
            var adminUser = "user2@user2.com";
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com", user1);
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            //Setup service for user2 as adminUser
            bool isAdmin=true;
            var service = _customerManagementTestHelper.SetupCustomersService(adminUser, isAdmin);

            //Act
            customerDto.Name = "Test12";
            //Update customer created by user1 as admin role user
            TestDelegate updateCustomer = () => service.Update(customerDto);

            //Assert
            Assert.DoesNotThrow(updateCustomer);
            Assert.IsTrue(Repository.Any(c => c.Name.Equals("Test12")));
        }

        [Test]
        public void AdminUserCanDeleteCustomerCreatedByAnotherUser()
        {
            //Arrange
            var user1 = "user1@user1.com";
            var adminUser = "user2@user2.com";
            var customerDto = _customerManagementTestHelper.SetupCustomerDto(1, "Test1", "asd@asd.com", user1);
            _customerManagementTestHelper.AddCustomer(customerDto);
            Assert.IsTrue(Repository.Any(c => c.Id == customerDto.Id));
            //Setup service for user2 as adminUser
            bool isAdmin = true;
            var service = _customerManagementTestHelper.SetupCustomersService(adminUser, isAdmin);

            //Act
            //Delete customer created by user1 as admin role user
            TestDelegate deleteCustomer = () => service.Delete(customerDto);

            //Assert
            Assert.DoesNotThrow(deleteCustomer);
            Assert.IsFalse(Repository.Any(c => c.Name.Equals(customerDto.Name)));
        }


        #endregion 


    }
}
