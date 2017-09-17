using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using CustomerManagement.API.Config;
using CustomerManagement.API.Constants;
using CustomerManagement.API.CustomExceptions;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Helpers;
using CustomerManagement.API.Interfaces;
using CustomerManagement.API.Models;
using CustomerManagement.API.Models.RequestResponseModels;
using CustomerManagement.Data;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;

namespace CustomerManagement.API.Services
{
    public class CustomersService : ICustomerService
    {
        private IRepository<Customer, CustomerModel> _repo;
        private readonly QueryResponseHelper _queryResponseHelper;
        private IHttpContextHelper _httpHelper;

        public CustomersService(IRepository<Customer,CustomerModel> customerRepository, IHttpContextHelper httpHelper)
        {
            _repo = customerRepository;
            _httpHelper = httpHelper;
            ApiMapperConfig.ConfigureMappings();
            _queryResponseHelper = new QueryResponseHelper(_httpHelper);
        }

        public GetCustomersResponseModel GetAll(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, bool>> @where = null; 
            Expression<Func<Customer, object>> sortBy = null; 

            @where = _queryResponseHelper.ComposeWhereCondition(query);

            @sortBy = _queryResponseHelper.ComposeSortByClause(query);


            var pageSize = query.PageSize == 0 ? ApplicationConstants.DefaultPageSize : query.PageSize;
            var pageNumber = query.PageNumber ==0? ApplicationConstants.DefaultPageNumber : query.PageNumber;

            var customers = _repo.Get(where: @where, orderBy: sortBy, orderAsc: query.SortAsc, pageSize: pageSize, pageNumber: pageNumber );

            var response = CreateGetCustomersResponseModel(query, customers, pageNumber);

            return response;
        }


        public CustomerDto GetById(int id)
        {
            var customer = _repo.Get(id);

            return Mapper.Map<CustomerDto>(customer);
        }

        public bool Update(CustomerDto customerDto)
        {
            try
            {
                var customerModel = _repo.Get(customerDto.Id);

                bool authorizedToUpdate = IsUserAuthorizedToUpdateOrDeleteCustomer(customerModel);

                if (authorizedToUpdate)
                {
                    customerModel = Mapper.Map<CustomerModel>(customerDto);

                    ValidateCustomerModel(customerModel);

                    return _repo.Update(customerModel) == 1;
                }

                string currentUserName = _httpHelper.CurrentUserName;
                throw new AuthorizationException($"User {currentUserName} is only authorized to update or delete records created by user {currentUserName}");
            }
            catch (Exception e)
            { //TODO : log exception
                throw e;
            }

        }

        public bool IsUserAuthorizedToUpdateOrDeleteCustomer(CustomerModel customerModel)
        {
            var currentUserName = _httpHelper.CurrentUserName;

            bool isAdmin = _httpHelper.IsCurrentUserRole(RolesEnum.Admin.ToString());

            if (!isAdmin && !currentUserName.Equals(customerModel.CreatedBy, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public CustomerDto Create(CustomerDto customer)
        {
            if (customer == null) return null;
            try
            {
                var customerModel = Mapper.Map<CustomerModel>(customer);

                customerModel.CreatedBy = _httpHelper.CurrentUserName;

                ValidateCustomerModel(customerModel);

                var addedCustomer = _repo.Add(customerModel);

                return Mapper.Map<CustomerDto>(addedCustomer);
            }
            catch (Exception e)
            {
                //TODO : log exception
                throw e;
            }
        }

        public void ValidateCustomerModel(CustomerModel customerModel)
        {
            if (string.IsNullOrWhiteSpace(customerModel.Name))
            {
                throw new BadRequestException("Customer Name is required");
            }

            if (string.IsNullOrWhiteSpace(customerModel.EmailAddress))
            {
                throw new BadRequestException("Customer Email address is required");
            }

            if (string.IsNullOrWhiteSpace(customerModel.EmailAddress))
            {
                throw new BadRequestException("Customer Email address is required");
            }

        }

        public bool Delete(CustomerDto customerDto)
        {
            try
            {
                var customerModel = _repo.Get(customerDto.Id);

                if (customerModel == null) return false;

                bool authorizedToDelete = IsUserAuthorizedToUpdateOrDeleteCustomer(customerModel);

                if (authorizedToDelete)
                {
                    return _repo.Delete(customerDto.Id) == 1;
                }

                string currentUserName = _httpHelper.CurrentUserName;
                throw new AuthorizationException($"User {currentUserName} is only authorized to update or delete records created by user {currentUserName}");

            }
            catch (Exception e)
            {
                //TODO : log exception
                throw e;
            }
        }

        private GetCustomersResponseModel CreateGetCustomersResponseModel(GetCustomersRequestModel query, ICollection<CustomerModel> customers, int pageNumber)
        {
            var response = new GetCustomersResponseModel();

            response.CustomerDtos = Mapper.Map<List<CustomerDto>>(customers.ToList());

            if (response.CustomerDtos.Count > 0)
            {
                response.NextPageUrl = _queryResponseHelper.ComposeGetUrl(query, pageNumber + 1);

                response.PreviousPageUrl = pageNumber > 1 ? _queryResponseHelper.ComposeGetUrl(query, pageNumber - 1) : null;
            }
            else
            {
                response.NextPageUrl = null;
                response.PreviousPageUrl = null;
            }
            return response;
        }
    }
}