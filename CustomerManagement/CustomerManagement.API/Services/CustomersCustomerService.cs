using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using AutoMapper;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Interfaces;
using CustomerManagement.API.Models.Request;
using CustomerManagement.Data;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;
using Microsoft.Ajax.Utilities;

namespace CustomerManagement.API.Services
{
    public class CustomersCustomerService : ICustomerService
    {
        private const string _baseApiUrl  = "api/Customers?";
        private const int _defaultPageSize = 10;
        private const int _defaultPageNumber = 1;


        private IRepository<Customer, CustomerModel> _repo;
        public CustomersCustomerService(IRepository<Customer,CustomerModel> customerRepository)
        {
            _repo = customerRepository;

        }

        public GetCustomersResponseModel GetAll(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, bool>> @where = null; 
            Expression<Func<Customer, object>> sortBy = null; 

            @where = ComposeWhereCondition(query);

            @sortBy = ComposeSortByClause(query);


            var pageSize = query.PageSize == 0 ? _defaultPageSize : query.PageSize;
            var pageNumber = query.PageNumber ==0? _defaultPageNumber : query.PageNumber;

            var customers = _repo.Get(where: @where, orderBy: sortBy, orderAsc: query.SortAsc, pageSize: pageSize, pageNumber: pageNumber );

            var response = new GetCustomersResponseModel();

            response.CustomerDtos = Mapper.Map<List<CustomerDto>>(customers.ToList());
            response.NextPageUrl = ComposeGetUrl(query, pageNumber + 1);

            if (pageNumber > 1)
            {
                response.PreviousPageUrl = ComposeGetUrl(query, pageNumber - 1);
            }
            else
            {
                response.PreviousPageUrl = string.Empty;
            }

            return response;
        }

        private string ComposeGetUrl(GetCustomersRequestModel query, int pageNumber)
        {
            string scheme = HttpContext.Current.Request.Url.Scheme;
            StringBuilder pageUrl = new StringBuilder(scheme+"://"+_baseApiUrl);
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                pageUrl.Append($"name={query.Name}");
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                pageUrl.Append($"&email={query.Email}");
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                pageUrl.Append($"&sortBy={query.SortBy}");
            }
            
            pageUrl.Append($"&sortAsc={query.SortAsc}");

            var pageSize = query.PageSize < 1 ? _defaultPageSize : query.PageSize;

            pageUrl.Append($"&pageSize={pageSize}");
            pageUrl.Append($"&pageNumber={pageNumber}");


            return pageUrl.ToString();
        }

        private static Expression<Func<Customer, object>> ComposeSortByClause(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, object>> sortBy = null;
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var sortByField = query.SortBy.ToLower();

                switch (sortByField)
                {
                    case "id":
                        sortBy = c => c.Id;
                        break;
                    case "name":
                        sortBy = c => c.Name;
                        break;
                    case "email":
                        sortBy = c => c.EmailAddress;
                        break;

                    default:
                        sortBy = c => c.Id;
                        break;
                }
            }

            return sortBy;
        }

        private Expression<Func<Customer, bool>> ComposeWhereCondition(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, bool>> @where = null;
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var name = query.Name.ToLower();
                if (!string.IsNullOrWhiteSpace(query.Email))
                {
                    var email = query.Email.ToLower();

                    @where = c => c.Name.ToLower().Contains(name)
                                  && c.EmailAddress.ToLower().Contains(email);
                }
                else
                {
                    @where = c => c.Name.ToLower().Contains(name);
                }
            }
            else if (!string.IsNullOrWhiteSpace(query.Email))
            {
                var email = query.Email.ToLower();

                @where = c => c.EmailAddress.ToLower().Contains(email);
            }

            return @where;
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
                return _repo.Update(Mapper.Map<CustomerModel>(customerDto)) == 1;
            }
            catch (Exception e)
            { //TODO : log exception
                throw e;
            }

        }

        public CustomerDto Create(CustomerDto customer)
        {
            try
            {
                var addedCustomer = _repo.Add(Mapper.Map<CustomerModel>(customer));

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
                return _repo.Delete(Mapper.Map<CustomerModel>(customerDto)) == 1;
            }
            catch (Exception e)
            {
                //TODO : log exception
                throw e;
            }
        }
    }
}