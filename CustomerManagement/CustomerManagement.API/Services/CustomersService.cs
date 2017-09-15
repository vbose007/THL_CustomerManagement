using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
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
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace CustomerManagement.API.Services
{
    public class CustomersService : ICustomerService
    {
        private IRepository<Customer, CustomerModel> _repo;
        private readonly QueryResponseHelper _queryResponseHelper;

        public CustomersService(IRepository<Customer,CustomerModel> customerRepository)
        {
            _repo = customerRepository;
            ApiMapperConfig.ConfigureMappings();
            _queryResponseHelper = new QueryResponseHelper();
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
                    return _repo.Update(Mapper.Map<CustomerModel>(customerDto)) == 1;
                }

                string currentUserName = HttpContext.Current.User.Identity.GetUserName();
                throw new AuthorizationException($"User {currentUserName} is only authorized to update or delete records created by user {currentUserName}");
            }
            catch (Exception e)
            { //TODO : log exception
                throw e;
            }

        }

        private bool IsUserAuthorizedToUpdateOrDeleteCustomer(CustomerModel customerModel)
        {
            var currentUserName = HttpContext.Current.User.Identity.GetUserName();

            bool isAdmin = HttpContext.Current.User.IsInRole(RolesEnum.Admin.ToString());

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

                customerModel.CreatedBy = HttpContext.Current.User.Identity.GetUserName();

                var addedCustomer = _repo.Add(customerModel);

                return Mapper.Map<CustomerDto>(addedCustomer);
            }
            catch (Exception e)
            {
                //TODO : log exception
                throw e;
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

                string currentUserName = HttpContext.Current.User.Identity.GetUserName();
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