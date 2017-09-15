using CustomerManagement.API.DTOs;
using CustomerManagement.API.Models.RequestResponseModels;

namespace CustomerManagement.API.Interfaces
{
    public interface ICustomerService
    {
        GetCustomersResponseModel GetAll(GetCustomersRequestModel query);
        CustomerDto GetById(int id);
        bool Update(CustomerDto customerDto);
        CustomerDto Create(CustomerDto customer);
        bool Delete(CustomerDto customerDto);
    }
}